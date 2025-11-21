import { Component, OnInit, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';

import { register } from 'swiper/element/bundle';
import { ProductService } from '../../services/product.services';

register();

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  template: `
    <section
      class="relative h-[600px] bg-gradient-to-r from-rose-100 to-pink-100"
    >
      <div class="container mx-auto px-4 h-full flex items-center">
        <div class="max-w-xl">
          <p class="text-sm font-medium tracking-widest mb-2 text-rose-600">
            NEW COLLECTION 2025
          </p>
          <h1 class="text-5xl font-bold mb-4">Timeless Elegance</h1>
          <p class="text-lg mb-8 text-gray-600">
            Discover curated pieces that blend romantic aesthetics
          </p>
          <a
            routerLink="/shop"
            class="inline-block bg-rose-600 hover:bg-rose-700 text-white px-8 py-3 rounded-full font-medium"
          >
            Shop Now
          </a>
        </div>
      </div>
    </section>

    <section class="container mx-auto px-4 py-20">
      <div class="text-center mb-12">
        <h2 class="text-4xl font-bold mb-3">Featured Products</h2>
        <p class="text-gray-600">Carefully selected items</p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
        @for (product of products; track product.id) {
        <div
          class="group card-hover bg-white rounded-lg overflow-hidden shadow-md cursor-pointer"
          [routerLink]="['/product', product.id]"
        >
          <div class="relative overflow-hidden aspect-[3/4]">
            <img
              [src]="product.imageUrl"
              [alt]="product.name"
              class="w-full h-full object-cover"
            />
          </div>
          <div class="p-5">
            <h3 class="text-lg font-semibold mb-1">{{ product.name }}</h3>
            <p class="text-sm text-gray-600 mb-3 line-clamp-2">
              {{ product.description }}
            </p>
            <span class="text-rose-600 font-bold text-xl"
              >\${{ product.price }}</span
            >
          </div>
        </div>
        }
      </div>
    </section>
  `,
})
export class HomeComponent implements OnInit {
  products: any[] = [];

  constructor(private productService: ProductService) {}

  ngOnInit() {
    this.productService.getProducts().subscribe((products) => {
      this.products = products.slice(0, 4);
    });
  }
}
