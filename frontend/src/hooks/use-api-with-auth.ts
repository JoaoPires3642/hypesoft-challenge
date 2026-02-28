"use client";

import { useSession } from "next-auth/react";
import * as api from "../services/api";

export function useApiWithAuth() {
  const { data: session } = useSession();
  const token = session?.accessToken;

  return {
    // Products
    fetchProducts: (filters?: Parameters<typeof api.fetchProducts>[0]) =>
      api.fetchProducts(filters, token),
    fetchProductById: (id: string) => api.fetchProductById(id, token),
    createProduct: (data: Parameters<typeof api.createProduct>[0]) =>
      api.createProduct(data, token),
    updateProduct: (
      id: string,
      data: Parameters<typeof api.updateProduct>[1]
    ) => api.updateProduct(id, data, token),
    deleteProduct: (id: string) => api.deleteProduct(id, token),

    // Categories
    fetchCategories: () => api.fetchCategories(token),
    fetchCategoryById: (id: string) => api.fetchCategoryById(id, token),
    createCategory: (data: Parameters<typeof api.createCategory>[0]) =>
      api.createCategory(data, token),
    updateCategory: (
      id: string,
      data: Parameters<typeof api.updateCategory>[1]
    ) => api.updateCategory(id, data, token),
    deleteCategory: (id: string) => api.deleteCategory(id, token),

    // Dashboard
    fetchDashboardData: () => api.fetchDashboardData(token),
    fetchDashboardKPIs: () => api.fetchDashboardKPIs(token),
    fetchCategoryDistribution: () => api.fetchCategoryDistribution(token),
    fetchLowStockProducts: () => api.fetchLowStockProducts(token),
  };
}
