
export interface Category {
  id: string;
  name: string;
  createdAt: string;
}

export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  categoryId: string;
  category?: Category;
  quantity: number;
  createdAt: string;
  updatedAt: string;
}

export interface ProductFormData {
  name: string;
  description: string;
  price: number;
  categoryId: string;
  quantity: number;
}

export interface CategoryFormData {
  name: string;
}

export interface DashboardKPIs {
  totalProducts: number;
  totalStockValue: number;
  lowStockAlerts: number;
}

export interface CategoryDistribution {
  name: string;
  value: number;
  fill: string;
}

export interface PaginatedResponse<T> {
  data: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ProductFilters {
  search?: string;
  categoryId?: string;
  page?: number;
  pageSize?: number;
}
