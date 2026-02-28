"use client";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";
import { useLowStockProducts } from "@/src/hooks/use-dashboard";
import { AlertTriangle } from "lucide-react";
import type { Product } from "@/src/types";

function getStockBadge(quantity: number) {
  if (quantity <= 2) {
    return (
      <Badge variant="destructive" className="text-xs">
        Crítico
      </Badge>
    );
  }
  if (quantity <= 5) {
    return (
      <Badge className="bg-warning/15 text-warning-foreground border-warning/30 text-xs">
        Baixo
      </Badge>
    );
  }
  return (
    <Badge variant="secondary" className="text-xs">
      Atenção
    </Badge>
  );
}

export function LowStockTable() {
  const { data, isLoading } = useLowStockProducts();
  
  const products = (data as Product[] | undefined) || [];

  if (isLoading) {
    return (
      <Card className="border-border/50 shadow-sm">
        <CardHeader>
          <Skeleton className="h-5 w-56" />
        </CardHeader>
        <CardContent>
          <div className="space-y-3">
            {Array.from({ length: 4 }).map((_, i) => (
              <Skeleton key={i} className="h-10 w-full" />
            ))}
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="border-border/50 shadow-sm">
      <CardHeader className="pb-3">
        <div className="flex items-center gap-2">
          <AlertTriangle className="h-4.5 w-4.5 text-warning" />
          <CardTitle className="text-base font-semibold text-card-foreground">
            Produtos com Estoque Baixo
          </CardTitle>
        </div>
      </CardHeader>
      <CardContent>
        {products && products.length > 0 ? (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Produto</TableHead>
                <TableHead>Categoria</TableHead>
                <TableHead className="text-center">Qtd.</TableHead>
                <TableHead className="text-right">Status</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {products.map((product) => (
                <TableRow key={product.id}>
                  <TableCell className="font-medium text-card-foreground">
                    {product.name}
                  </TableCell>
                  <TableCell className="text-muted-foreground">
                    {product.category?.name ?? "—"}
                  </TableCell>
                  <TableCell className="text-center font-mono text-sm font-semibold text-card-foreground">
                    {product.quantity}
                  </TableCell>
                  <TableCell className="text-right">
                    {getStockBadge(product.quantity)}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        ) : (
          <p className="py-8 text-center text-sm text-muted-foreground">
            Nenhum produto com estoque baixo.
          </p>
        )}
      </CardContent>
    </Card>
  );
}
