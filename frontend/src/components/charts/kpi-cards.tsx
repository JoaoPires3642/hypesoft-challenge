"use client";

import { Package, DollarSign, AlertTriangle, AlertCircle } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { useDashboardKPIs } from "@/src/hooks/use-dashboard";
import type { DashboardKPIs } from "@/src/types";

function formatCurrency(value: number): string {
  return new Intl.NumberFormat("pt-BR", {
    style: "currency",
    currency: "BRL",
  }).format(value);
}

export function KpiCards() {
  const { data, isLoading, error } = useDashboardKPIs();
  
  const kpiData = (data as DashboardKPIs | undefined);

  if (isLoading) {
    return (
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {Array.from({ length: 3 }).map((_, i) => (
          <Card key={i} className="border-border/50 shadow-sm">
            <CardContent className="p-6">
              <div className="flex items-center justify-between">
                <Skeleton className="h-4 w-28" />
                <Skeleton className="h-10 w-10 rounded-xl" />
              </div>
              <Skeleton className="mt-3 h-8 w-36" />
              <Skeleton className="mt-2 h-3 w-24" />
            </CardContent>
          </Card>
        ))}
      </div>
    );
  }

  if (error) {
    return (
      <Card className="border-destructive/50 bg-destructive/5">
        <CardContent className="flex items-center gap-3 p-6">
          <AlertCircle className="h-5 w-5 text-destructive" />
          <div>
            <p className="font-medium text-destructive">
              Erro ao carregar métricas
            </p>
            <p className="text-sm text-muted-foreground">
              Tente recarregar a página
            </p>
          </div>
        </CardContent>
      </Card>
    );
  }

  const kpis = [
    {
      label: "Total de Produtos",
      value: kpiData?.totalProducts ?? 0,
      format: (v: number) => v.toString(),
      icon: Package,
      color: "bg-primary/10 text-primary",
      change: "+3 este mês",
    },
    {
      label: "Valor em Estoque",
      value: kpiData?.totalStockValue ?? 0,
      format: formatCurrency,
      icon: DollarSign,
      color: "bg-success/10 text-success",
      change: "Atualizado agora",
    },
    {
      label: "Alertas de Estoque",
      value: kpiData?.lowStockAlerts ?? 0,
      format: (v: number) => v.toString(),
      icon: AlertTriangle,
      color: "bg-warning/10 text-warning",
      change: "Requer atenção",
    },
  ];

  return (
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
      {kpis.map((kpi) => (
        <Card
          key={kpi.label}
          className="border-border/50 shadow-sm transition-shadow hover:shadow-md"
        >
          <CardContent className="p-6">
            <div className="flex items-center justify-between">
              <p className="text-sm font-medium text-muted-foreground">
                {kpi.label}
              </p>
              <div
                className={`flex h-10 w-10 items-center justify-center rounded-xl ${kpi.color}`}
              >
                <kpi.icon className="h-5 w-5" />
              </div>
            </div>
            <p className="mt-2 text-2xl font-bold tracking-tight text-card-foreground">
              {kpi.format(kpi.value)}
            </p>
            <p className="mt-1 text-xs text-muted-foreground">{kpi.change}</p>
          </CardContent>
        </Card>
      ))}
    </div>
  );
}
