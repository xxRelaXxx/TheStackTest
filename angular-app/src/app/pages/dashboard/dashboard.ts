import { Component, signal, inject, AfterViewInit, ElementRef, ViewChild, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Chart, BarController, BarElement, CategoryScale, LinearScale, Tooltip, ChartConfiguration } from 'chart.js';
import { ApiService } from '../../services/api.service';
import { DashboardStats } from '../../models';

Chart.register(BarController, BarElement, CategoryScale, LinearScale, Tooltip);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
})
export class DashboardComponent implements AfterViewInit, OnDestroy {
  @ViewChild('barChart') barChartRef!: ElementRef<HTMLCanvasElement>;

  private api = inject(ApiService);
  stats = signal<DashboardStats | null>(null);
  loading = signal(true);
  error = signal('');
  private chart: Chart | null = null;

  ngAfterViewInit() {
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    this.error.set('');
    this.api.getDashboardStats().subscribe({
      next: data => {
        this.stats.set(data);
        this.loading.set(false);
        setTimeout(() => this.buildChart(data), 0);
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Impossibile caricare i dati. Verifica che il backend sia in esecuzione.');
      }
    });
  }

  private buildChart(data: DashboardStats) {
    if (this.chart) {
      this.chart.destroy();
      this.chart = null;
    }
    if (!this.barChartRef?.nativeElement) return;

    const labels = data.ultimiDodiciMesi.map(m => m.mese);
    const values = data.ultimiDodiciMesi.map(m => m.valore);

    this.chart = new Chart(this.barChartRef.nativeElement, {
      type: 'bar',
      data: {
        labels,
        datasets: [{
          data: values,
          backgroundColor: 'rgba(99, 102, 241, 0.85)',
          hoverBackgroundColor: 'rgba(129, 140, 248, 1)',
          borderRadius: 6,
          borderSkipped: false,
        }]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false },
          tooltip: {
            callbacks: {
              label: ctx => ` € ${(ctx.raw as number).toLocaleString('it-IT', { minimumFractionDigits: 2 })}`
            }
          }
        },
        scales: {
          x: {
            grid: { color: 'rgba(255,255,255,0.05)' },
            ticks: { color: 'rgba(255,255,255,0.4)', font: { size: 11 } }
          },
          y: {
            grid: { color: 'rgba(255,255,255,0.05)' },
            ticks: { color: 'rgba(255,255,255,0.4)', font: { size: 11 } }
          }
        }
      }
    } as ChartConfiguration);
  }

  formatCurrency(val: number) {
    return val.toLocaleString('it-IT', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  trendClass(pct: number) {
    return pct >= 0 ? 'bg-green-500/20 text-green-400' : 'bg-red-500/20 text-red-400';
  }

  trendIcon(pct: number) {
    return pct >= 0 ? '↑' : '↓';
  }

  ngOnDestroy() {
    this.chart?.destroy();
  }
}
