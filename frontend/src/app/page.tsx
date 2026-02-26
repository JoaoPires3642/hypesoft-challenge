"use client";

import { KpiCards } from "@/src/components/charts/kpi-cards";
import { CategoryChart } from "@/src/components/charts/category-chart";
import { LowStockTable } from "@/src/components/charts/low-stock-table";

export default function DashboardPage() {
  return (
    <div className="space-y-6">
      {/* Page header */}
      <div>
        <h1 className="text-2xl font-bold tracking-tight text-foreground">
          Dashboard
        </h1>
        <p className="text-sm text-muted-foreground">
          Visão geral do seu inventário e métricas de estoque.
        </p>
      </div>

      {/* KPIs */}
      <KpiCards />

      {/* Charts & Tables */}
      <div className="grid gap-6 lg:grid-cols-2">
        <CategoryChart />
        <LowStockTable />
      </div>
    </div>
  );
}
