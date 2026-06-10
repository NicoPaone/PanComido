import { RecetaIngrediente } from '../../../core/models/domain/plato';

export function calcularCostoReceta(receta: RecetaIngrediente[]): number {
  return receta.reduce((total, ingrediente) => {
    const costoUnitario = ingrediente.costoUnitario ?? 0;
    return total + costoUnitario * ingrediente.cantidad;
  }, 0);
}
