"use client";

import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { useSession } from "next-auth/react";
import type { CategoryFormData } from "@/src/types";
import { PRODUCTS_KEY } from "./use-products";
import { useApiWithAuth } from "./use-api-with-auth";

export const CATEGORIES_KEY = "categories";

export function useCategories() {
  const api = useApiWithAuth();
  const { data: session, status } = useSession();
  
  return useQuery({
    queryKey: [CATEGORIES_KEY],
    queryFn: api.fetchCategories,
    enabled: status === "authenticated" && !!session?.accessToken,
    retry: 2,
    staleTime: 30000, // 30 segundos
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
