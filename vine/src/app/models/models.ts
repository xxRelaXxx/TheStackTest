export interface Booking {
  id: string;
  nome: string;
  cognome: string;
  email: string;
  telefono: string;
  tipoPrenotazione: string[];
  data: Date;
  numeroPersone: number;
  messaggio?: string;
  status: BookingStatus;
  createdAt: Date;
}

export enum BookingStatus {
  NEW = 'new',
  CONFIRMED = 'confirmed',
  ARCHIVED = 'archived',
  REJECTED = 'rejected'
}

export interface Product {
  id: string;
  nome: string;
  descrizione: string;
  prezzo: number;
  amazonUrl: string;
  immagine: string;
  categoria: string;
}
