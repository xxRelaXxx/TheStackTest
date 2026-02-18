import { Component, ChangeDetectionStrategy, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
  host: {
    '(mousemove)': 'onMouseMove($event)'
  }
})
export class Home {
  protected readonly router = inject(Router);
  private readonly productService = inject(ProductService);
  
  protected readonly parallaxX = signal(0);
  protected readonly parallaxY = signal(0);
  protected readonly featuredProducts = this.productService.products;
  
  protected onMouseMove(event: MouseEvent): void {
    const x = (event.clientX / window.innerWidth - 0.5) * 20;
    const y = (event.clientY / window.innerHeight - 0.5) * 20;
    this.parallaxX.set(x);
    this.parallaxY.set(y);
  }
  
  protected navigateToContact(params: any): void {
    this.router.navigate(['/contattaci'], { queryParams: params });
  }
}
