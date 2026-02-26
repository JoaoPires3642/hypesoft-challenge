"use client";

import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  fetchProducts,
  createProduct,
  updateProduct,
  deleteProduct,
} from "@/src/services/api";
import type { ProductFilters, ProductFormData } from "@/src/types";

export const PRODUCTS_KEY = "products";

export function useProducts(filters?: ProductFilters) {
  return useQuery({
    queryKey: [PRODUCTS_KEY, filters],
    queryFn: () => fetchProducts(filters),
  });
}

export function useCreateProduct() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: ProductFormData) => createProduct(data),
    onSuccess: () => {
      queryClient.removeQueries({ queryKey: [PRODUCTS_KEY] });
      queryClient.invalidateQueries({ 
        queryKey: ["dashboard"],
        refetchType: 'active'
      });
    },
  });
}

export function useUpdateProduct() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: ProductFormData }) =>
      updateProduct(id, data),
    onSuccess: () => {
      queryClient.removeQueries({ queryKey: [PRODUCTS_KEY] });
      queryClient.invalidateQueries({ 
        queryKey: ["dashboard"],
        refetchType: 'active'
      });
    },
  });
}

export function useDeleteProduct() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteProduct(id),
    onSuccess: () => {

      queryClient.removeQueries({ queryKey: [PRODUCTS_KEY] });
      queryClient.invalidateQueries({ 
        queryKey: ["dashboard"],
        refetchType: 'active'
      });
    },
  });
}


