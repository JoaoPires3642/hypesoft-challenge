"use client";

import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  fetchCategories,
  createCategory,
  deleteCategory,
} from "@/src/services/api";
import type { CategoryFormData } from "@/src/types";
import { PRODUCTS_KEY } from "./use-products";

export const CATEGORIES_KEY = "categories";

export function useCategories() {
  return useQuery({
    queryKey: [CATEGORIES_KEY],
    queryFn: fetchCategories,
  });
}

export function useCreateCategory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (data: CategoryFormData) => createCategory(data),
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
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteCategory(id),
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
