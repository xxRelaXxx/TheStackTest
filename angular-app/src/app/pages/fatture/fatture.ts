import { Component, signal, inject, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ApiService } from '../../services/api.service';
import { Fattura, Cliente, Tema } from '../../models';

@Component({
  selector: 'app-fatture',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './fatture.html',
})
export class FattureComponent implements OnInit {
  private api = inject(ApiService);

  allFatture = signal<Fattura[]>([]);
  clienti = signal<Cliente[]>([]);
  temi = signal<Tema[]>([]);
  loading = signal(false);
  error = signal('');

  // Filter state
  showFilterModal = signal(false);
  filterClienteId = signal<number | null>(null);
  filterTemaId = signal<number | null>(null);
  activeFilterClienteId = signal<number | null>(null);
  activeFilterTemaId = signal<number | null>(null);

  // Detail modal
  detailFattura = signal<Fattura | null>(null);

  // Multi-select
  selectedEntrateIds = signal<Set<number>>(new Set());
  selectedUsciteIds = signal<Set<number>>(new Set());

  // Computed
  entrate = computed(() =>
    this.allFatture().filter(f =>
      f.tipoFattura === 'Entrata' &&
      (!this.activeFilterClienteId() || f.clienteId === this.activeFilterClienteId()) &&
      (!this.activeFilterTemaId() || f.temaId === this.activeFilterTemaId())
    )
  );

  uscite = computed(() =>
    this.allFatture().filter(f =>
      f.tipoFattura === 'Uscita' &&
      (!this.activeFilterClienteId() || f.clienteId === this.activeFilterClienteId()) &&
      (!this.activeFilterTemaId() || f.temaId === this.activeFilterTemaId())
    )
  );

  ngOnInit() {
    this.loadAll();
  }

  loadAll() {
    this.loading.set(true);
    this.error.set('');
    this.api.getFatture().subscribe({
      next: data => { this.allFatture.set(data); this.loading.set(false); },
      error: () => { this.error.set('Errore nel caricamento delle fatture.'); this.loading.set(false); }
    });
    this.api.getClienti().subscribe({ next: d => this.clienti.set(d) });
    this.api.getTemi().subscribe({ next: d => this.temi.set(d) });
  }

  openFilter() { this.showFilterModal.set(true); }
  closeFilter() { this.showFilterModal.set(false); }

  applyFilter() {
    this.activeFilterClienteId.set(this.filterClienteId());
    this.activeFilterTemaId.set(this.filterTemaId());
    this.closeFilter();
  }

  clearFilter() {
    this.filterClienteId.set(null);
    this.filterTemaId.set(null);
    this.activeFilterClienteId.set(null);
    this.activeFilterTemaId.set(null);
  }

  isFiltered() { return !!this.activeFilterClienteId() || !!this.activeFilterTemaId(); }

  deleteFattura(id: number) {
    if (!confirm('Eliminare questa fattura?')) return;
    this.api.deleteFattura(id).subscribe({
      next: () => this.allFatture.update(arr => arr.filter(f => f.id !== id)),
      error: () => alert('Errore durante l\'eliminazione.')
    });
  }

  // Multi-select for Entrate
  toggleEntrataSelect(id: number) {
    this.selectedEntrateIds.update(s => {
      const next = new Set(s);
      next.has(id) ? next.delete(id) : next.add(id);
      return next;
    });
  }
  isEntrataSelected(id: number) { return this.selectedEntrateIds().has(id); }
  allEntrateSelected = computed(() =>
    this.entrate().length > 0 && this.entrate().every(f => this.selectedEntrateIds().has(f.id))
  );
  toggleAllEntrate() {
    if (this.allEntrateSelected()) {
      this.selectedEntrateIds.set(new Set());
    } else {
      this.selectedEntrateIds.set(new Set(this.entrate().map(f => f.id)));
    }
  }
  deleteSelectedEntrate() {
    const ids = [...this.selectedEntrateIds()];
    if (!ids.length || !confirm(`Eliminare ${ids.length} fatture selezionate?`)) return;
    Promise.all(ids.map(id => this.api.deleteFattura(id).toPromise())).then(() => {
      this.allFatture.update(arr => arr.filter(f => !ids.includes(f.id)));
      this.selectedEntrateIds.set(new Set());
    });
  }

  // Multi-select for Uscite
  toggleUscitaSelect(id: number) {
    this.selectedUsciteIds.update(s => {
      const next = new Set(s);
      next.has(id) ? next.delete(id) : next.add(id);
      return next;
    });
  }
  isUscitaSelected(id: number) { return this.selectedUsciteIds().has(id); }
  allUsciteSelected = computed(() =>
    this.uscite().length > 0 && this.uscite().every(f => this.selectedUsciteIds().has(f.id))
  );
  toggleAllUscite() {
    if (this.allUsciteSelected()) {
      this.selectedUsciteIds.set(new Set());
    } else {
      this.selectedUsciteIds.set(new Set(this.uscite().map(f => f.id)));
    }
  }
  deleteSelectedUscite() {
    const ids = [...this.selectedUsciteIds()];
    if (!ids.length || !confirm(`Eliminare ${ids.length} fatture selezionate?`)) return;
    Promise.all(ids.map(id => this.api.deleteFattura(id).toPromise())).then(() => {
      this.allFatture.update(arr => arr.filter(f => !ids.includes(f.id)));
      this.selectedUsciteIds.set(new Set());
    });
  }

  openDetail(f: Fattura) { this.detailFattura.set(f); }
  closeDetail() { this.detailFattura.set(null); }

  formatDate(d: string) {
    return new Date(d).toLocaleDateString('it-IT');
  }

  formatCurrency(val: number, simbolo = '€') {
    return `${simbolo} ${val.toLocaleString('it-IT', { minimumFractionDigits: 2 })}`;
  }

  clienteInitials(nome: string) {
    return nome.split(' ').map(p => p[0]).join('').substring(0, 2).toUpperCase();
  }

  statusLabel(f: Fattura) {
    const due = new Date(f.dataScadenza);
    const now = new Date();
    return due < now ? 'scaduta' : 'pagato';
  }

  statusClass(f: Fattura) {
    const due = new Date(f.dataScadenza);
    const now = new Date();
    return due < now
      ? 'bg-red-500/20 text-red-400'
      : 'bg-green-500/20 text-green-400';
  }
}
