"use client";

import { useQuery } from "@tanstack/react-query";
import {
  fetchDashboardKPIs,
  fetchCategoryDistribution,
  fetchLowStockProducts,
} from "@/src/services/api";

export function useDashboardKPIs() {
  return useQuery({
    queryKey: ["dashboard", "kpis"],
    queryFn: fetchDashboardKPIs,
  });
}

export function useCategoryDistribution() {
  return useQuery({
    queryKey: ["dashboard", "distribution"],
    queryFn: fetchCategoryDistribution,
  });
}

export function useLowStockProducts() {
  return useQuery({
    queryKey: ["dashboard", "low-stock"],
    queryFn: fetchLowStockProducts,
  });
}
