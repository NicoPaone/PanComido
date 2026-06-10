import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'arsCurrency',
  standalone: true,
})
export class ArsCurrencyPipe implements PipeTransform {
  private readonly formatter = new Intl.NumberFormat('es-AR', {
    style: 'currency',
    currency: 'ARS',
    minimumFractionDigits: 0,
    maximumFractionDigits: 2,
  });

  transform(value: number | string | null | undefined): string {
    const amount = Number(value);
    if (!Number.isFinite(amount)) return 'Sin precio';

    return this.formatter.format(amount);
  }
}
