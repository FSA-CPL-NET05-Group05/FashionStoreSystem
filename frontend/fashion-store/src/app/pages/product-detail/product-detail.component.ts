import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { forkJoin } from 'rxjs';
import { StockService } from '../../services/stock.service';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-product-detail',
  imports: [RouterModule],
  templateUrl: '../product-detail/product-detail.component.html',
})
export class ProductDetailComponent implements OnInit {
  product: any = null;
  selectedImage = '';
  currentStock = 0;
  quantity = 1;

  selectedSize: any = null;
  selectedColor: any = null;

  availableSizes: any[] = [];
  availableColors: any[] = [];

  feedbacks: any[] = [];
  averageRating = 0;

  route = inject(ActivatedRoute);
  stockService = inject(StockService);
  productService = inject(ProductService);

  ngOnInit(): void {
    window.scrollTo(0, 0);
    const id = +this.route.snapshot.params['id'];
    this.loadProductDetail(id);
  }

  loadProductDetail(id: number) {
    forkJoin({
      product: this.productService.getProduct(id),
      productSizes: this.productService.getProductSizes(id),
      sizes: this.productService.getSizes(),
      colors: this.productService.getColors(),
    }).subscribe({
      next: ({ product, productSizes, sizes, colors }) => {
        this.product = product;
        this.selectedImage = product.imageUrl;

        const uniqueSizeIds = [
          ...new Set(productSizes.map((ps: any) => ps.sizeId)),
        ];
        this.availableSizes = sizes.filter((s) => uniqueSizeIds.includes(s.id));

        const uniqueColorIds = [
          ...new Set(productSizes.map((ps: any) => ps.colorId)),
        ];
        this.availableColors = colors.filter((c) =>
          uniqueColorIds.includes(c.id)
        );

        if (this.availableSizes.length)
          this.selectedSize = this.availableSizes[0];
        if (this.availableColors.length)
          this.selectedColor = this.availableColors[0];

        this.updateStock();
      },
    });
  }

  selectSize(size: any) {
    this.selectedSize = size;
    this.updateStock();
  }

  selectColor(color: any) {
    this.selectedColor = color;
    this.updateStock();
  }

  updateStock() {
    if (!this.selectedSize || !this.selectedColor) return;

    this.stockService
      .getStock(this.product.id, this.selectedSize.id, this.selectedColor.id)
      .subscribe((productSize) => {
        this.currentStock = productSize?.stock || 0;
        if (this.quantity > this.currentStock) {
          this.quantity = Math.max(1, this.currentStock);
        }
      });
  }
}
