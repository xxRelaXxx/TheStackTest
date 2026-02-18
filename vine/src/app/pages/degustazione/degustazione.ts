import { Component, ChangeDetectionStrategy, signal, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { BookingService } from '../../services/booking.service';

interface CalendarDay {
  day: number;
  date: Date | null;
}

interface CalendarMonth {
  name: string;
  days: CalendarDay[];
}

@Component({
  selector: 'app-degustazione',
  imports: [ReactiveFormsModule],
  templateUrl: './degustazione.html',
  styleUrl: './degustazione.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Degustazione {
  private readonly fb = inject(FormBuilder);
  private readonly bookingService = inject(BookingService);
  
  protected readonly submitted = signal(false);
  protected readonly selectedDate = signal<Date | null>(null);
  
  protected readonly bookingForm = this.fb.group({
    tipologia: ['', Validators.required],
    orario: ['', Validators.required],
    nome: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    telefono: ['', Validators.required],
    messaggio: ['']
  });
  
  // Generate calendar for next 5 months
  protected readonly displayMonths: CalendarMonth[] = this.generateCalendarMonths();
  protected readonly currentMonthIndex = signal<number>(0);

  protected visibleMonth(): CalendarMonth {
    return this.displayMonths[this.currentMonthIndex()];
  }

  protected prevMonth(): void {
    const idx = this.currentMonthIndex();
    if (idx > 0) this.currentMonthIndex.set(idx - 1);
  }

  protected nextMonth(): void {
    const idx = this.currentMonthIndex();
    if (idx < this.displayMonths.length - 1) this.currentMonthIndex.set(idx + 1);
  }
  
  private generateCalendarMonths(): CalendarMonth[] {
    const months: CalendarMonth[] = [];
    const today = new Date();
    const monthNames = ['Gennaio', 'Febbraio', 'Marzo', 'Aprile', 'Maggio', 'Giugno', 
                        'Luglio', 'Agosto', 'Settembre', 'Ottobre', 'Novembre', 'Dicembre'];
    
    for (let i = 0; i < 5; i++) {
      const date = new Date(today.getFullYear(), today.getMonth() + i, 1);
      const monthName = monthNames[date.getMonth()];
      const days = this.generateMonthDays(date);
      
      months.push({
        name: monthName,
        days
      });
    }
    
    return months;
  }
  
  private generateMonthDays(monthDate: Date): CalendarDay[] {
    const days: CalendarDay[] = [];
    const year = monthDate.getFullYear();
    const month = monthDate.getMonth();
    
    // Get first day of month and number of days in month
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    const daysInMonth = lastDay.getDate();
    const startingDayOfWeek = firstDay.getDay();
    
    // Add empty cells for days before month starts
    for (let i = 0; i < startingDayOfWeek; i++) {
      days.push({ day: 0, date: null });
    }
    
    // Add days of the month (excluding Sundays)
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    for (let day = 1; day <= daysInMonth; day++) {
      const date = new Date(year, month, day);
      const dayOfWeek = date.getDay();
      
      // Include all days except Sundays and past dates
      if (dayOfWeek !== 0 && date >= today) {
        days.push({ day, date });
      } else {
        days.push({ day: 0, date: null });
      }
    }
    
    return days;
  }
  
  protected selectDate(date: Date | null): void {
    if (date) {
      this.selectedDate.set(date);
    }
  }
  
  protected isDateSelected(date: Date | null): boolean {
    if (!date) return false;
    const selected = this.selectedDate();
    if (!selected) return false;
    return date.toDateString() === selected.toDateString();
  }
  
  protected onSubmit(): void {
    if (this.bookingForm.valid && this.selectedDate()) {
      const formValue = this.bookingForm.value;
      
      this.bookingService.addBooking({
        nome: formValue.nome!,
        cognome: '',
        email: formValue.email!,
        telefono: formValue.telefono!,
        tipoPrenotazione: [formValue.tipologia || 'Degustazione'],
        data: this.selectedDate()!,
        numeroPersone: 1,
        messaggio: formValue.messaggio || undefined
      });
      
      this.submitted.set(true);
    }
  }
}