import { Component, effect, inject, signal } from '@angular/core';
import { MozoHubService } from '../../../../core/services/hubs/llamados/mozo-hub-service';
import { AuthService } from '../../../../core/services/auth.service';
import { LlamadoService } from '../../../../core/services/llamados/llamado-service';
import { Llamado } from '../../../../core/models/llamados/llamado';
import { LlamadoCard } from '../../components/llamado-card/llamado-card';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-llamados-mozo-page',
  imports: [LlamadoCard],
  templateUrl: './llamados-mozo-page.html',
  styleUrl: './llamados-mozo-page.css',
})
export class LlamadosMozoPage {

   private llamadoService = inject(LlamadoService);
  private hub = inject(MozoHubService);
 // private auth = inject(AuthService);

  pendientes = signal<Llamado[]>([]);
  cargando = signal<boolean>(true);
  error = signal<string | null>(null);
  resolviendoId = signal<number | null>(null);  // para deshabilitar el botón mientras se procesa


  private mozoId = 3 ;
  private restauranteId = 1;

  private subs = new Subscription();

  constructor() {
    // Reaccionar a cada llamado nuevo que llega por SignalR
    effect(() => {
      const nuevo = this.hub.llamadoRecibido();
      if (!nuevo) return;

      // Lo agrego arriba de la lista (orden por más reciente)
      this.pendientes.update((lista) => [nuevo, ...lista]);

      // Feedback al usuario (lo podés cambiar por un toast service)
      console.log(`[SignalR] Nuevo llamado: ${nuevo.descripcion}`);
    });
  }

  ngOnInit(): void {
    this.cargarPendientes();
    this.conectarAlHub();
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
   
  }

  // ====== Carga inicial HTTP ======
  private cargarPendientes(): void {
    this.cargando.set(true);
    this.error.set(null);

    this.subs.add(
      this.llamadoService
        .listarPendientesDelMozo(this.mozoId)
        .subscribe({
          next: (lista) => {
            this.pendientes.set(lista);
            this.cargando.set(false);
          },
          error: (err) => {
            this.error.set('No pudimos cargar los llamados. Reintentá.');
            this.cargando.set(false);
            console.error('[LlamadosMozo] Error cargando pendientes:', err);
          },
        }),
    );
  }

  // ====== Conexión SignalR ======
  private async conectarAlHub(): Promise<void> {
    try {
      await this.hub.conectarYUnirseGrupo(this.restauranteId, this.mozoId);
    } catch (err) {
      console.error('[LlamadosMozo] Error conectando al hub:', err);
      this.error.set('No pudimos conectar al sistema de notificaciones.');
    }
  }

  // ====== Resolver llamado ======
  resolverLlamado(llamadoId: number): void {
    if (this.resolviendoId() !== null) return;   // evita doble click

    this.resolviendoId.set(llamadoId);

    this.subs.add(
      this.llamadoService.resolver(llamadoId).subscribe({
        next: () => {
          // Lo saco de la lista localmente (optimista)
          this.pendientes.update((lista) =>
            lista.filter((l) => l.id !== llamadoId),
          );
          this.resolviendoId.set(null);
        },
        error: (err) => {
          this.resolviendoId.set(null);
          this.error.set('No pudimos marcar el llamado como resuelto.');
          console.error('[LlamadosMozo] Error resolviendo:', err);
        },
      }),
    );
  }

  // ====== Reintento ======
  reintentar(): void {
    this.cargarPendientes();
  }
}