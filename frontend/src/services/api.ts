
import type {
  Product,
  ProductFormData,
  Category,
  CategoryFormData,
  ProductFilters,
  PaginatedResponse,
  DashboardKPIs,
  CategoryDistribution,
} from "@/src/types";

const CHART_COLORS = [
  "var(--color-chart-1)",
  "var(--color-chart-2)",
  "var(--color-chart-3)",
  "var(--color-chart-4)",
  "var(--color-chart-5)",
];

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://api:5000";

// ----- PRODUTOS -----

export async function fetchProducts(
  filters?: ProductFilters
): Promise<PaginatedResponse<Product>> {
  try {
    const params = new URLSearchParams();
    if (filters?.search) params.append("search", filters.search);
    if (filters?.categoryId) params.append("categoryId", filters.categoryId);
    if (filters?.page) params.append("page", filters.page.toString());
    if (filters?.pageSize) params.append("pageSize", filters.pageSize.toString());

    const response = await fetch(`${API_BASE_URL}/api/products?${params}`);
    if (!response.ok) throw new Error("Falha ao buscar produtos");
    const data = await response.json();

    // Mapear stockQuantity (backend) para quantity (frontend)
    const items = (data.items || []).map((item: any) => ({
      ...item,
      quantity: item.stockQuantity,
    }));

    return {
      data: items,
      total: data.total || 0,
      page: data.pageIndex || 1,
      pageSize: data.pageSize || 10,
      totalPages: Math.ceil((data.total || 0) / (data.pageSize || 10)),
    };
  } catch (error) {
    console.error("Erro ao buscar produtos:", error);
    return {
      data: [],
      total: 0,
      page: 1,
      pageSize: 10,
      totalPages: 0,
    };
  }
}

export async function fetchProductById(id: string): Promise<Product | null> {
  try {
    const response = await fetch(`${API_BASE_URL}/api/products/${id}`);
    if (response.status === 404) return null;
    if (!response.ok) throw new Error("Falha ao buscar produto");
    const data = await response.json();
    // Mapear stockQuantity (backend) para quantity (frontend)
    return {
      ...data,
      quantity: data.stockQuantity,
    };
  } catch (error) {
    console.error("Erro ao buscar produto:", error);
    return null;
  }
}

export async function createProduct(data: ProductFormData): Promise<Product> {
  try {
    const payload = {
      name: data.name,
      description: data.description,
      price: data.price,
      stockQuantity: data.quantity,
      categoryId: data.categoryId,
    };
    
    const response = await fetch(`${API_BASE_URL}/api/products`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
    if (!response.ok) throw new Error("Falha ao criar produto");
    const productId = await response.json();
    return {
      id: productId,
      ...data,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };
  } catch (error) {
    console.error("Erro ao criar produto:", error);
    throw error;
  }
}

export async function updateProduct(
  id: string,
  data: ProductFormData
): Promise<Product> {
  try {
    // Mapear quantity (frontend) para stockQuantity (backend)
    const payload = {
      id,
      name: data.name,
      description: data.description,
      price: data.price,
      stockQuantity: data.quantity,
      categoryId: data.categoryId,
    };
    
    const response = await fetch(`${API_BASE_URL}/api/products/${id}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });
    if (response.status === 404) throw new Error("Produto não encontrado");
    if (!response.ok) throw new Error("Falha ao atualizar produto");
    return {
      id,
      ...data,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
    };
  } catch (error) {
    console.error("Erro ao atualizar produto:", error);
    throw error;
  }
}

export async function deleteProduct(id: string): Promise<void> {
  try {
    const response = await fetch(`${API_BASE_URL}/api/products/${id}`, {
      method: "DELETE",
    });
    if (response.status === 404) throw new Error("Produto não encontrado");
    if (!response.ok) throw new Error("Falha ao deletar produto");
  } catch (error) {
    console.error("Erro ao deletar produto:", error);
    throw error;
  }
}

// ----- CATEGORIAS -----

export async function fetchCategories(): Promise<Category[]> {
  try {
    const response = await fetch(`${API_BASE_URL}/api/categories`);
    if (!response.ok) throw new Error("Falha ao buscar categorias");
    const data = await response.json();
    const items = Array.isArray(data) ? data : [];
    return items.map((cat: any) => ({
      ...cat,
      createdAt: cat.createdAt || new Date().toISOString(),
    }));
  } catch (error) {
    console.error("Erro ao buscar categorias:", error);
    return [];
  }
}

export async function createCategory(data: CategoryFormData): Promise<Category> {
  try {
    const response = await fetch(`${API_BASE_URL}/api/categories`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(data),
    });
    if (!response.ok) throw new Error("Falha ao criar categoria");
    const categoryId = await response.json();
    return {
      id: categoryId,
      name: data.name,
      createdAt: new Date().toISOString(),
    };
  } catch (error) {
    console.error("Erro ao criar categoria:", error);
    throw error;
  }
}

export async function deleteCategory(id: string): Promise<void> {
  try {
    const response = await fetch(`${API_BASE_URL}/api/categories/${id}`, {
      method: "DELETE",
    });
    if (response.status === 404) throw new Error("Categoria não encontrada");
    if (!response.ok) throw new Error("Falha ao deletar categoria");
  } catch (error) {
    console.error("Erro ao deletar categoria:", error);
    throw error;
  }
}

// ----- DASHBOARD -----

export async function fetchDashboardSummary() {
  try {
    const response = await fetch(`${API_BASE_URL}/api/dashboard`);
    if (!response.ok) throw new Error("Falha ao buscar dashboard");
    return await response.json();
  } catch (error) {
    console.error("Erro ao buscar dashboard:", error);
    // Fallback para dados mock em caso de erro
    return {
      totalProducts: 0,
      totalStockValue: 0,
      lowStockProducts: [],
      productsByCategory: [],
    };
  }
}

export async function fetchDashboardKPIs(): Promise<DashboardKPIs> {
  const summary = await fetchDashboardSummary();
  
  const lowStockAlerts = summary.lowStockProducts?.length || 0;

  return {
    totalProducts: summary.totalProducts || 0,
    totalStockValue: summary.totalStockValue || 0,
    lowStockAlerts,
  };
}

export async function fetchCategoryDistribution(): Promise<
  CategoryDistribution[]
> {
  const summary = await fetchDashboardSummary();
  
  return (summary.productsByCategory || []).map((cat: any, i: number) => ({
    name: cat.categoryName || "Sem categoria",
    value: cat.productCount || 0,
    fill: CHART_COLORS[i % CHART_COLORS.length],
  }));
}

export async function fetchLowStockProducts(): Promise<Product[]> {
  const summary = await fetchDashboardSummary();
  
  return (summary.lowStockProducts || []).map((p: any) => ({
    id: p.id,
    name: p.name,
    description: p.description,
    price: p.price,
    quantity: p.stockQuantity,
    categoryId: p.categoryId,
    createdAt: p.createdAt || new Date().toISOString(),
    updatedAt: p.updatedAt || new Date().toISOString(),
  }));
}
