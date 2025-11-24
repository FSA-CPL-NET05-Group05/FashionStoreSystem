export interface User {
  id: string;
  username: string;
  password: string;
  email: string;
  role: 'admin' | 'customer';
  status: 'active' | 'banned';
  fullName: string;
  phone: string;
}

export interface Category {
  id: number;
  name: string;
}

export interface Size {
  id: number;
  name: string;
}

export interface Color {
  id: number;
  name: string;
  code: string;
}

export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  categoryId: number;
}

export interface ProductSize {
  id: number;
  productId: number;
  sizeId: number;
  colorId: number;
  stock: number;
}

export interface CartItem {
  id: number;
  userId: string;
  productId: number;
  sizeId: number;
  colorId: number;
  quantity: number;
  addedAt: string;
}

export interface Order {
  id: number;
  orderDate: string;
  userId?: string;
  totalAmount: number;
  status: string;
  guestName?: string;
  guestEmail?: string;
  guestPhone?: string;
}

export interface OrderDetail {
  id: number;
  orderId: number;
  productId: number;
  quantity: number;
  price: number;
  sizeId: number;
  colorId: number;
}

export interface Feedback {
  id: number;
  userId: string;
  productId: number;
  comment: string;
  rating: number;
  createdDate: string;
  adminReply?: string;
}

export interface PurchaseHistory {
  id: number;
  userId: string;
  productId: number;
  orderId: number;
}
