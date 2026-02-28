"use client";

import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useSession } from "next-auth/react";
import type { ProductFilters, ProductFormData } from "@/src/types";
import { useApiWithAuth } from "./use-api-with-auth";

export const PRODUCTS_KEY = "products";

export function useProducts(filters?: ProductFilters) {
  const api = useApiWithAuth();
  const { data: session, status } = useSession();
  
  return useQuery({
    queryKey: [PRODUCTS_KEY, filters],
    queryFn: () => api.fetchProducts(filters),
    enabled: status === "authenticated" && !!session?.accessToken,
    retry: 2,
    staleTime: 30000, // 30 segundos
  });
}

export function useCreateProduct() {
  const api = useApiWithAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: api.createProduct,
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
  const api = useApiWithAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: ProductFormData }) =>
      api.updateProduct(id, data),
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
  const api = useApiWithAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: api.deleteProduct,
    onSuccess: () => {

      queryClient.removeQueries({ queryKey: [PRODUCTS_KEY] });
      queryClient.invalidateQueries({ 
        queryKey: ["dashboard"],
        refetchType: 'active'
      });
    },
  });
}


