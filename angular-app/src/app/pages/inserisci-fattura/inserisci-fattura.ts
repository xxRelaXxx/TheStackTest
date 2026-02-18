import { Component, signal, computed, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ApiService } from '../../services/api.service';
import {
  Cliente, TipoPagamento, Assoggettamento, Tema, Valuta, Arrotondamento
} from '../../models';

type ModalType = 'cliente' | 'pagamento' | 'assoggettamento' | 'tema' | 'valuta' | 'arrotondamento' | null;

@Component({
  selector: 'app-inserisci-fattura',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './inserisci-fattura.html',
})
export class InserisciFatturaComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);

  // Lookup data
  clienti = signal<Cliente[]>([]);
  tipiPagamento = signal<TipoPagamento[]>([]);
  assoggettamenti = signal<Assoggettamento[]>([]);
  temi = signal<Tema[]>([]);
  valute = signal<Valuta[]>([]);
  arrotondamenti = signal<Arrotondamento[]>([]);

  // Form fields
  clienteId = signal<number | null>(null);
  tipoFattura = 'Entrata';
  metodoPagamentoId = signal<number | null>(null);
  numeroFattura = '';
  dataEmissione = '';
  dataScadenza = '';
  assoggettamentoId = signal<number | null>(null);
  temaId = signal<number | null>(null);
  imponibileNetto = signal<number | null>(null);
  valutaId = signal<number | null>(null);
  arrotondamentoId = signal<number | null>(null);

  // Computed total
  totaleLordo = computed(() => {
    const netto = this.imponibileNetto() ?? 0;
    const ass = this.assoggettamenti().find(a => a.id === this.assoggettamentoId());
    if (!ass) return netto;
    return netto + (netto * ass.percentuale / 100);
  });

  // State
  loading = signal(false);
  success = signal(false);
  errors = signal<Record<string, string>>({});
  activeModal = signal<ModalType>(null);

  // Add-new form fields
  newClienteNome = '';
  newClienteCF = '';
  newClientePIVA = '';
  newClienteIBAN = '';
  newClienteEmail = '';
  newPagamentoNome = '';
  newAssoggettamentoNome = '';
  newAssoggettamentoPerc = 0;
  newTemaNome = '';
  newValutaCodice = '';
  newValutaNome = '';
  newValutaSimbolo = '€';
  newArrotondamentoNome = '';
  addingNew = signal(false);

  ngOnInit() {
    this.loadLookups();
    // Default date to today
    this.dataEmissione = new Date().toISOString().split('T')[0];
    const d = new Date(); d.setDate(d.getDate() + 30);
    this.dataScadenza = d.toISOString().split('T')[0];
  }

  loadLookups() {
    this.api.getClienti().subscribe({ next: d => this.clienti.set(d) });
    this.api.getTipiPagamento().subscribe({ next: d => this.tipiPagamento.set(d) });
    this.api.getAssoggettamenti().subscribe({ next: d => this.assoggettamenti.set(d) });
    this.api.getTemi().subscribe({ next: d => this.temi.set(d) });
    this.api.getValute().subscribe({ next: d => this.valute.set(d) });
    this.api.getArrotondamenti().subscribe({ next: d => this.arrotondamenti.set(d) });
  }

  openModal(type: ModalType) { this.activeModal.set(type); this.addingNew.set(false); }
  closeModal() { this.activeModal.set(null); }

  // ─── Add new items ────────────────────────────────────────────────────────
  addCliente() {
    if (!this.newClienteNome.trim()) return;
    this.addingNew.set(true);
    this.api.createCliente({
      nome: this.newClienteNome, codiceFiscale: this.newClienteCF,
      partitaIVA: this.newClientePIVA, iban: this.newClienteIBAN, email: this.newClienteEmail
    }).subscribe({
      next: c => {
        this.clienti.update(a => [...a, c]);
        this.clienteId.set(c.id);
        this.newClienteNome = this.newClienteCF = this.newClientePIVA = this.newClienteIBAN = this.newClienteEmail = '';
        this.addingNew.set(false); this.closeModal();
      },
      error: () => { this.addingNew.set(false); alert('Errore nella creazione del cliente.'); }
    });
  }

  addPagamento() {
    if (!this.newPagamentoNome.trim()) return;
    this.addingNew.set(true);
    this.api.createTipoPagamento(this.newPagamentoNome).subscribe({
      next: p => {
        this.tipiPagamento.update(a => [...a, p]);
        this.metodoPagamentoId.set(p.id);
        this.newPagamentoNome = '';
        this.addingNew.set(false); this.closeModal();
      },
      error: () => { this.addingNew.set(false); alert('Errore.'); }
    });
  }

  addAssoggettamento() {
    if (!this.newAssoggettamentoNome.trim()) return;
    this.addingNew.set(true);
    this.api.createAssoggettamento(this.newAssoggettamentoNome, this.newAssoggettamentoPerc).subscribe({
      next: a => {
        this.assoggettamenti.update(arr => [...arr, a]);
        this.assoggettamentoId.set(a.id);
        this.newAssoggettamentoNome = ''; this.newAssoggettamentoPerc = 0;
        this.addingNew.set(false); this.closeModal();
      },
      error: () => { this.addingNew.set(false); alert('Errore.'); }
    });
  }

  addTema() {
    if (!this.newTemaNome.trim()) return;
    this.addingNew.set(true);
    this.api.createTema(this.newTemaNome).subscribe({
      next: t => {
        this.temi.update(a => [...a, t]);
        this.temaId.set(t.id);
        this.newTemaNome = '';
        this.addingNew.set(false); this.closeModal();
      },
      error: () => { this.addingNew.set(false); alert('Errore.'); }
    });
  }

  addValuta() {
    if (!this.newValutaCodice.trim()) return;
    this.addingNew.set(true);
    this.api.createValuta(this.newValutaCodice, this.newValutaNome, this.newValutaSimbolo).subscribe({
      next: v => {
        this.valute.update(a => [...a, v]);
        this.valutaId.set(v.id);
        this.newValutaCodice = this.newValutaNome = ''; this.newValutaSimbolo = '€';
        this.addingNew.set(false); this.closeModal();
      },
      error: () => { this.addingNew.set(false); alert('Errore.'); }
    });
  }

  addArrotondamento() {
    if (!this.newArrotondamentoNome.trim()) return;
    this.addingNew.set(true);
    this.api.createArrotondamento(this.newArrotondamentoNome).subscribe({
      next: a => {
        this.arrotondamenti.update(arr => [...arr, a]);
        this.arrotondamentoId.set(a.id);
        this.newArrotondamentoNome = '';
        this.addingNew.set(false); this.closeModal();
      },
      error: () => { this.addingNew.set(false); alert('Errore.'); }
    });
  }

  // ─── Validation ───────────────────────────────────────────────────────────
  validate(): boolean {
    const errs: Record<string, string> = {};
    if (!this.clienteId()) errs['cliente'] = 'Seleziona un cliente';
    if (!this.tipoFattura) errs['tipo'] = 'Seleziona il tipo di fattura';
    if (!this.metodoPagamentoId()) errs['pagamento'] = 'Seleziona un metodo di pagamento';
    if (!this.numeroFattura.trim()) errs['numero'] = 'Inserisci il numero fattura';
    if (!this.dataEmissione) errs['emissione'] = 'Inserisci la data di emissione';
    if (!this.dataScadenza) errs['scadenza'] = 'Inserisci la data di scadenza';
    if (!this.assoggettamentoId()) errs['assoggettamento'] = 'Seleziona un assoggettamento IVA';
    if (!this.temaId()) errs['tema'] = 'Seleziona un argomento';
    if (!this.imponibileNetto() && this.imponibileNetto() !== 0) errs['netto'] = 'Inserisci l\'imponibile netto';
    if (!this.valutaId()) errs['valuta'] = 'Seleziona una valuta';
    this.errors.set(errs);
    return Object.keys(errs).length === 0;
  }

  submit() {
    if (!this.validate()) return;
    this.loading.set(true);
    this.api.createFattura({
      numeroFattura: this.numeroFattura,
      tipoFattura: this.tipoFattura,
      dataEmissione: new Date(this.dataEmissione).toISOString(),
      dataScadenza: new Date(this.dataScadenza).toISOString(),
      imponibileNetto: this.imponibileNetto()!,
      clienteId: this.clienteId()!,
      metodoPagamentoId: this.metodoPagamentoId()!,
      assoggettamentoId: this.assoggettamentoId()!,
      temaId: this.temaId()!,
      valutaId: this.valutaId()!,
      arrotondamentoId: this.arrotondamentoId() ?? undefined
    }).subscribe({
      next: () => {
        this.loading.set(false);
        this.success.set(true);
        setTimeout(() => this.router.navigate(['/fatture']), 1500);
      },
      error: () => {
        this.loading.set(false);
        alert('Errore durante l\'inserimento della fattura.');
      }
    });
  }

  hasError(field: string) { return !!this.errors()[field]; }
  getError(field: string) { return this.errors()[field] || ''; }
}
