import { Injectable, signal } from '@angular/core';
// EmailJS client library (install with `npm install @emailjs/browser`)
import emailjs from '@emailjs/browser';

export interface ContactRequest {
  id: string;
  nome: string;
  cognome: string;
  email: string;
  telefono: string;
  degustazione: boolean;
  notteConNoi: boolean;
  visitaTomba: boolean;
  messaggio?: string;
  createdAt: Date;
}

@Injectable({ providedIn: 'root' })
export class ContactService {
  private readonly STORAGE_KEY = 'vino_contact_requests_v1';

  private readonly requests = signal<ContactRequest[]>(this.loadFromStorage());

  readonly allRequests = this.requests;

  addRequest(payload: Omit<ContactRequest, 'id' | 'createdAt'>) {
    const req: ContactRequest = {
      ...payload,
      id: Math.random().toString(36).substring(2, 9),
      createdAt: new Date()
    };

    this.requests.update(list => {
      const updated = [...list, req];
      try {
        localStorage.setItem(this.STORAGE_KEY, JSON.stringify(updated.map(r => ({ ...r, createdAt: r.createdAt.toString() }))));
      } catch (e) {
        console.warn('Failed saving contact requests', e);
      }
      return updated;
    });

    return req;
  }

  private loadFromStorage(): ContactRequest[] {
    try {
      const raw = localStorage.getItem(this.STORAGE_KEY);
      if (!raw) return [];
      const parsed = JSON.parse(raw) as any[];
      return parsed.map(p => ({ ...p, createdAt: p.createdAt ? new Date(p.createdAt) : new Date() }));
    } catch (e) {
      console.warn('Failed loading contact requests', e);
      return [];
    }
  }

  async sendEmailViaServer(req: ContactRequest): Promise<Response> {
    // Attempts to POST to /api/send-email. Requires server to be running.
    return fetch('/api/send-email', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(req)
    });
  }

  /**
   * Send using EmailJS (client-side). Configure these IDs in your environment.
   * See: https://www.emailjs.com/docs/sdk/installation/
   */
  async sendViaEmailJS(req: ContactRequest): Promise<{ ok: boolean; info?: any }>
  {
    try {
      const serviceId = (window as any).__EMAILJS_SERVICE_ID || 'your_service_id';
      const templateId = (window as any).__EMAILJS_TEMPLATE_ID || 'your_template_id';
      const publicKey = (window as any).__EMAILJS_PUBLIC_KEY || 'your_public_key';

      // Prepare template params according to your EmailJS template variables
      const templateParams = {
        to_email: req.email,
        nome: req.nome,
        cognome: req.cognome,
        telefono: req.telefono,
        messaggio: req.messaggio || '',
        interessi: [req.degustazione ? 'Degustazione' : null, req.notteConNoi ? 'Notte con noi' : null, req.visitaTomba ? 'Visita tomba' : null].filter(Boolean).join(', '),
      };

      const result = await emailjs.send(serviceId, templateId, templateParams, publicKey);
      return { ok: true, info: result };
    } catch (e) {
      console.warn('EmailJS send failed', e);
      return { ok: false, info: e };
    }
  }
}
