export interface JwtPayload {
  sub: string;
  name: string;
  email: string;
  role: string;
  restauranteId: string;
  exp: number;
}