import { Component, ChangeDetectionStrategy, signal, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ContactService } from '../../services/contact.service';

@Component({
  selector: 'app-contattaci',
  imports: [ReactiveFormsModule],
  templateUrl: './contattaci.html',
  styleUrl: './contattaci.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Contattaci implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly contactService = inject(ContactService);
  
  protected readonly submitted = signal(false);
  
  protected readonly contactForm = this.fb.group({
    nome: ['', Validators.required],
    cognome: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    telefono: ['', Validators.required],
    degustazione: [false],
    notteConNoi: [false],
    visitaTomba: [false],
    messaggio: ['']
  });
  
  ngOnInit(): void {
    // Pre-fill form based on query parameters
    this.route.queryParams.subscribe(params => {
      if (params['degustazione'] === 'true') {
        this.contactForm.patchValue({ degustazione: true });
      }
      if (params['notteConNoi'] === 'true') {
        this.contactForm.patchValue({ notteConNoi: true });
      }
      if (params['visitaTomba'] === 'true') {
        this.contactForm.patchValue({ visitaTomba: true });
      }
    });
  }
  
  protected onSubmit(): void {
    if (this.contactForm.valid) {
      const form = this.contactForm.value;

      const req = this.contactService.addRequest({
        nome: form.nome!,
        cognome: form.cognome!,
        email: form.email!,
        telefono: form.telefono!,
        degustazione: !!form.degustazione,
        notteConNoi: !!form.notteConNoi,
        visitaTomba: !!form.visitaTomba,
        messaggio: form.messaggio || ''
      });

      // Prefer EmailJS client-side send if configured, otherwise fall back to server endpoint
      this.contactService.sendViaEmailJS(req)
        .then(result => {
          if (!result.ok) {
            // fallback to server if EmailJS failed
            return this.contactService.sendEmailViaServer(req).catch(e => {
              console.warn('Fallback server send failed', e);
              return undefined;
            });
          }
          return undefined;
        })
        .catch(err => {
          console.warn('Both EmailJS and server send failed', err);
        })
        .finally(() => this.submitted.set(true));
    }
  }
}
