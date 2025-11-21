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
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  products: any[] = [];

  heroSlides = [
    {
      id: 1,
      title: 'Timeless Elegance',
      subtitle: 'NEW COLLECTION 2025',
      description:
        'Discover curated pieces that blend romantic aesthetics with modern sophistication',
      image:
        'https://images.unsplash.com/photo-1483985988355-763728e1935b?w=1200',
      buttonText: 'Shop Now',
    },
    {
      id: 2,
      title: 'Modern Sophistication',
      subtitle: 'SPRING ESSENTIALS',
      description:
        'Embrace the season with our refined collection of contemporary classics',
      image:
        'https://images.unsplash.com/photo-1490481651871-ab68de25d43d?w=1200',
      buttonText: 'Explore Collection',
    },
    {
      id: 3,
      title: 'Effortless Style',
      subtitle: 'MINIMALIST WARDROBE',
      description: 'Timeless pieces designed for the modern lifestyle',
      image:
        'https://images.unsplash.com/photo-1441986300917-64674bd600d8?w=1200',
      buttonText: 'Discover More',
    },
  ];

  constructor(private productService: ProductService) {}

  ngOnInit() {
    this.productService.getProducts().subscribe((products) => {
      this.products = products.slice(0, 4);
    });
  }

  trackById(index: number, item: any) {
    return item.id;
  }
}
