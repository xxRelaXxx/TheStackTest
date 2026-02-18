import { Component, ChangeDetectionStrategy, signal, computed, inject } from '@angular/core';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-shop',
  imports: [],
  templateUrl: './shop.html',
  styleUrl: './shop.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class Shop {
  private readonly productService = inject(ProductService);
  
  protected readonly allProducts = this.productService.products;
  protected readonly selectedCategory = signal<string>('Tutti');
  
  protected readonly categories = computed(() => {
    const cats = new Set(this.allProducts().map(p => p.categoria));
    return ['Tutti', ...Array.from(cats)];
  });
  
  protected readonly filteredProducts = computed(() => {
    const category = this.selectedCategory();
    if (category === 'Tutti') {
      return this.allProducts();
    }
    return this.allProducts().filter(p => p.categoria === category);
  });
  
  protected selectCategory(category: string): void {
    this.selectedCategory.set(category);
  }
}
