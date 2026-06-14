export interface PlatoRendimientoDto {
  Nombre?: string;
  nombre?: string;
  Unidades?: string;
  unidades?: string;
  Facturacion?: string;
  facturacion?: string;
}

export interface DashboardRendimientoResponseDto {
  MasVendidos?: PlatoRendimientoDto[];
  masVendidos?: PlatoRendimientoDto[];
  MenosVendidos?: PlatoRendimientoDto[];
  menosVendidos?: PlatoRendimientoDto[];
}
