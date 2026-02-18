export interface Account {
  id: number;
  username: string;
  email: string;
  isAdmin: boolean;
  created?: string;
}

export interface Cliente {
  id: number;
  nome: string;
  codiceFiscale?: string;
  partitaIVA?: string;
  iban?: string;
  email?: string;
  telefono?: string;
  indirizzo?: string;
}

export interface TipoPagamento {
  id: number;
  nome: string;
}

export interface Assoggettamento {
  id: number;
  nome: string;
  percentuale: number;
}

export interface Tema {
  id: number;
  nome: string;
  colore?: string;
}

export interface Valuta {
  id: number;
  codice: string;
  nome?: string;
  simbolo: string;
}

export interface Arrotondamento {
  id: number;
  nome: string;
}

export interface Fattura {
  id: number;
  numeroFattura: string;
  tipoFattura: string;
  dataEmissione: string;
  dataScadenza: string;
  imponibileNetto: number;
  totaleLordo: number;
  clienteId: number;
  cliente: Cliente;
  metodoPagamentoId: number;
  metodoPagamento: TipoPagamento;
  assoggettamentoId: number;
  assoggettamento: Assoggettamento;
  temaId: number;
  tema: Tema;
  valutaId: number;
  valuta: Valuta;
  arrotondamentoId?: number;
  arrotondamento?: Arrotondamento;
}

export interface DashboardStats {
  profitto: number;
  percentualeProfitto: number;
  entrate: number;
  percentualeEntrate: number;
  ultimiDodiciMesi: { mese: string; valore: number }[];
}

export interface LoginResponse {
  token: string;
  user: Account;
}

export interface FatturaRequest {
  numeroFattura: string;
  tipoFattura: string;
  dataEmissione: string;
  dataScadenza: string;
  imponibileNetto: number;
  clienteId: number;
  metodoPagamentoId: number;
  assoggettamentoId: number;
  temaId: number;
  valutaId: number;
  arrotondamentoId?: number;
}
