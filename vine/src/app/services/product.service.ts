import { Injectable, signal } from '@angular/core';
import { Product } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  // Sample products data
  readonly products = signal<Product[]>([
    {
      id: '1',
      nome: 'Chianti Classico DOCG',
      descrizione: 'Vino rosso corposo, invecchiato in botti di rovere',
      prezzo: 18.50,
      amazonUrl: 'https://www.amazon.it',
      immagine: '/images/bottle-green.png',
      categoria: 'Rossi'
    },
    {
      id: '2',
      nome: 'Brunello di Montalcino',
      descrizione: 'Grande vino rosso toscano, elegante e strutturato',
      prezzo: 45.00,
      amazonUrl: 'https://www.amazon.it',
      immagine: '/images/bottle-yellow.png',
      categoria: 'Rossi'
    },
    {
      id: '3',
      nome: 'Vernaccia di San Gimignano',
      descrizione: 'Vino bianco fresco e minerale',
      prezzo: 12.50,
      amazonUrl: 'https://www.amazon.it',
      immagine: '/images/bottle-cyan.png',
      categoria: 'Bianchi'
    },
    {
      id: '4',
      nome: 'Rosso di Montepulciano',
      descrizione: 'Vino rosso giovane e fruttato',
      prezzo: 15.00,
      amazonUrl: 'https://www.amazon.it',
      immagine: '/images/bottle-green.png',
      categoria: 'Rossi'
    },
    {
      id: '5',
      nome: 'Vin Santo',
      descrizione: 'Vino dolce da dessert, perfetto con cantucci',
      prezzo: 25.00,
      amazonUrl: 'https://www.amazon.it',
      immagine: '/images/bottle-yellow.png',
      categoria: 'Dolci'
    },
    {
      id: '6',
      nome: 'Olio Extra Vergine',
      descrizione: 'Olio extravergine di oliva prodotto nel nostro podere',
      prezzo: 20.00,
      amazonUrl: 'https://www.amazon.it',
      immagine: '/images/bottle-cyan.png',
      categoria: 'Altri prodotti'
    }
  ]);
}
