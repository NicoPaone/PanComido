export interface ProductoStockMock {
    id: string;
    nombre:string;
    stock: number;
    unidadMedida: ['KG', 'GR'];
    fechaVencimiento:Date;
    stockMinimo: number;
    categoriaIngrediente: ['Verdura','Almacen','Carne'];

}