"use client";

import { useQuery } from "@tanstack/react-query";
import { useApiWithAuth } from "./use-api-with-auth";

export function useDashboardKPIs() {
  const api = useApiWithAuth();
  return useQuery({
    queryKey: ["dashboard", "kpis"],
    queryFn: api.fetchDashboardKPIs,
  });
}

export function useCategoryDistribution() {
  const api = useApiWithAuth();
  return useQuery({
    queryKey: ["dashboard", "distribution"],
    queryFn: api.fetchCategoryDistribution,
  });
}

export function useLowStockProducts() {
  const api = useApiWithAuth();
  return useQuery({
    queryKey: ["dashboard", "low-stock"],
    queryFn: api.fetchLowStockProducts,
  });
}
