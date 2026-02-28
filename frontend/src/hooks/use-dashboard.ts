"use client";

import { useQuery } from "@tanstack/react-query";
import { useSession } from "next-auth/react";
import { useApiWithAuth } from "./use-api-with-auth";

export function useDashboardKPIs() {
  const api = useApiWithAuth();
  const { data: session, status } = useSession();
  
  return useQuery({
    queryKey: ["dashboard", "kpis"],
    queryFn: api.fetchDashboardKPIs,
    enabled: status === "authenticated" && !!session?.accessToken,
    retry: 2,
    staleTime: 30000, // 30 segundos
  });
}

export function useCategoryDistribution() {
  const api = useApiWithAuth();
  const { data: session, status } = useSession();
  
  return useQuery({
    queryKey: ["dashboard", "distribution"],
    queryFn: api.fetchCategoryDistribution,
    enabled: status === "authenticated" && !!session?.accessToken,
    retry: 2,
    staleTime: 30000,
  });
}

export function useLowStockProducts() {
  const api = useApiWithAuth();
  const { data: session, status } = useSession();
  
  return useQuery({
    queryKey: ["dashboard", "low-stock"],
    queryFn: api.fetchLowStockProducts,
    enabled: status === "authenticated" && !!session?.accessToken,
    retry: 2,
    staleTime: 30000,
  });
}
