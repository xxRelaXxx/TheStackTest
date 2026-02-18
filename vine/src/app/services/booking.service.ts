import { Injectable, signal, computed } from '@angular/core';
import { Booking, BookingStatus } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class BookingService {
  // Persisted storage key
  private readonly STORAGE_KEY = 'vino_bookings_v1';

  // In-memory storage for bookings (persisted to localStorage so admin and page reloads see them)
  private readonly bookings = signal<Booking[]>(this.loadFromStorage());
  
  // Computed signals for filtered bookings
  readonly newBookings = computed(() => 
    this.bookings().filter(b => b.status === BookingStatus.NEW)
  );
  
  readonly confirmedBookings = computed(() => 
    this.bookings().filter(b => b.status === BookingStatus.CONFIRMED)
  );
  
  readonly archivedBookings = computed(() => 
    this.bookings().filter(b => b.status === BookingStatus.ARCHIVED || b.status === BookingStatus.REJECTED)
  );
  
  addBooking(booking: Omit<Booking, 'id' | 'status' | 'createdAt'>): void {
    const newBooking: Booking = {
      ...booking,
      id: Math.random().toString(36).substring(2, 9),
      status: BookingStatus.NEW,
      createdAt: new Date()
    };

    // Update in-memory signal and persist
    this.bookings.update(bookings => {
      const updated = [...bookings, newBooking];
      this.saveToStorage(updated);
      return updated;
    });

    // Try to notify the user by opening the user's email client with a pre-filled message
    // (Note: automatic server-side email sending requires a backend or third-party API.)
    this.openMailClient(newBooking);
  }

  private saveToStorage(list: Booking[]): void {
    try {
      const serializable = list.map(b => ({ ...b, data: b.data?.toString(), createdAt: b.createdAt?.toString() }));
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(serializable));
    } catch (e) {
      console.warn('Failed to save bookings to localStorage', e);
    }
  }

  private loadFromStorage(): Booking[] {
    try {
      const raw = localStorage.getItem(this.STORAGE_KEY);
      if (!raw) {
        // default demo data if nothing in storage
        return [
          {
            id: '1',
            nome: 'Mario',
            cognome: 'Rossi',
            email: 'mario.rossi@email.it',
            telefono: '+39 333 1234567',
            tipoPrenotazione: ['Degustazione Standard'],
            data: new Date('2026-03-15'),
            numeroPersone: 2,
            messaggio: 'Vorremmo una visita guidata della cantina',
            status: BookingStatus.NEW,
            createdAt: new Date('2026-02-01')
          }
        ];
      }

      const parsed = JSON.parse(raw) as any[];
      return parsed.map(p => ({
        ...p,
        data: p.data ? new Date(p.data) : new Date(),
        createdAt: p.createdAt ? new Date(p.createdAt) : new Date()
      } as Booking));
    } catch (e) {
      console.warn('Failed to load bookings from localStorage', e);
      return [];
    }
  }

  private openMailClient(booking: Booking): void {
    try {
      const subject = encodeURIComponent(`Conferma prenotazione - ${booking.nome} ${booking.cognome}`);
      const body = encodeURIComponent(
        `Ciao ${booking.nome},%0D%0A%0D%0AGrazie per la prenotazione per il giorno ${booking.data.toLocaleDateString('it-IT')}.
%0D%0A\nTi contatteremo a breve per i dettagli.%0D%0A%0D%0ACordiali saluti,%0D%0AIl team`
      );
      const mailto = `mailto:${booking.email}?subject=${subject}&body=${body}`;
      // open in a new tab to trigger the user's mail client
      window.open(mailto, '_blank');
    } catch (e) {
      console.warn('Failed to open mail client for booking', e);
    }
  }
  
  confirmBooking(id: string): void {
    this.bookings.update(bookings => {
      const updated = bookings.map(b => b.id === id ? { ...b, status: BookingStatus.CONFIRMED } : b);
      this.saveToStorage(updated);
      return updated;
    });
  }
  
  rejectBooking(id: string): void {
    this.bookings.update(bookings => {
      const updated = bookings.map(b => b.id === id ? { ...b, status: BookingStatus.REJECTED } : b);
      this.saveToStorage(updated);
      return updated;
    });
  }
  
  archiveBooking(id: string): void {
    this.bookings.update(bookings => {
      const updated = bookings.map(b => b.id === id ? { ...b, status: BookingStatus.ARCHIVED } : b);
      this.saveToStorage(updated);
      return updated;
    });
  }
}
