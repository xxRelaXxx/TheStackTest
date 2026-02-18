import { Component, ChangeDetectionStrategy, signal, inject } from '@angular/core';
import { BookingService } from '../../services/booking.service';
import { Booking } from '../../models/models';
import { ContactService } from '../../services/contact.service';

type TabType = 'new' | 'confirmed' | 'archived' | 'richieste';

@Component({
  selector: 'app-admin',
  imports: [],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Admin {
  private readonly bookingService = inject(BookingService);
  private readonly contactService = inject(ContactService);
  
  protected readonly activeTab = signal<TabType>('new');
  protected readonly newBookings = this.bookingService.newBookings;
  protected readonly confirmedBookings = this.bookingService.confirmedBookings;
  protected readonly archivedBookings = this.bookingService.archivedBookings;
  protected readonly contactRequests = this.contactService.allRequests;
  
  protected selectTab(tab: TabType): void {
    this.activeTab.set(tab);
  }
  
  protected confirmBooking(id: string): void {
    if (confirm('Confermare questa prenotazione?')) {
      this.bookingService.confirmBooking(id);
    }
  }
  
  protected rejectBooking(id: string): void {
    if (confirm('Rifiutare questa prenotazione? Questa azione la sposter\u00e0 negli archiviate.')) {
      this.bookingService.rejectBooking(id);
    }
  }
  
  protected formatDate(date: Date): string {
    return new Date(date).toLocaleDateString('it-IT', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }
  
  protected formatDateTime(date: Date): string {
    return new Date(date).toLocaleDateString('it-IT', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
