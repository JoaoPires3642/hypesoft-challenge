"use client";

import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import type { CategoryFormData } from "@/src/types";
import { PRODUCTS_KEY } from "./use-products";
import { useApiWithAuth } from "./use-api-with-auth";

export const CATEGORIES_KEY = "categories";

export function useCategories() {
  const api = useApiWithAuth();
  return useQuery({
    queryKey: [CATEGORIES_KEY],
    queryFn: api.fetchCategories,
  });
}

export function useCreateCategory() {
  const api = useApiWithAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: api.createCategory,
    onSuccess: () => {
      queryClient.invalidateQueries({ 
        queryKey: [CATEGORIES_KEY],
        refetchType: 'active'
      });
      queryClient.invalidateQueries({ 
        queryKey: [PRODUCTS_KEY],
        refetchType: 'active'
      });
      queryClient.invalidateQueries({ 
        queryKey: ["dashboard"],
        refetchType: 'active'
      });
    },
  });
}

export function useDeleteCategory() {
  const api = useApiWithAuth();
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: api.deleteCategory,
    onSuccess: () => {
      queryClient.invalidateQueries({ 
        queryKey: [CATEGORIES_KEY],
        refetchType: 'active'
      });
      queryClient.invalidateQueries({ 
        queryKey: ["dashboard"],
        refetchType: 'active'
      });
    },
  });
}
