"use client";

import { ProductsDataTable } from "@/src/components/forms/products-data-table";

export default function ProdutosPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold tracking-tight text-foreground">
          Produtos
        </h1>
        <p className="text-sm text-muted-foreground">
          Gerencie o catálogo de produtos do seu estoque.
        </p>
      </div>

      <ProductsDataTable />
    </div>
  );
}