
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

// API Base URL - must be set via NEXT_PUBLIC_API_URL environment variable
// Fallback to http://api:5000 for docker container communication
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://api:5000";

if (typeof window !== 'undefined' && !process.env.NEXT_PUBLIC_API_URL) {
  console.warn('NEXT_PUBLIC_API_URL is not set. Using fallback URL.');
}

// Helper function to get auth headers
function getAuthHeaders(token?: string): HeadersInit {
  const headers: HeadersInit = {
    "Content-Type": "application/json",
  };
  
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }
  
  return headers;
}

export class ApiError extends Error {
  status: number;
  detail?: string;

  constructor(message: string, status: number, detail?: string) {
    super(message);
    this.status = status;
    this.detail = detail;
  }
}

type ErrorResponseBody = {
  message?: string;
  title?: string;
  error?: string;
  detail?: string;
  errors?: Record<string, string[] | string>;
};

async function parseErrorDetail(response: Response): Promise<string | undefined> {
  const contentType = response.headers.get("content-type") || "";

  if (contentType.includes("application/json")) {
    try {
      const body = (await response.json()) as ErrorResponseBody | string;
      if (typeof body === "string") return body.trim() || undefined;

      if (body.detail) return body.detail;
      if (body.message) return body.message;
      if (body.title) return body.title;
      if (body.error) return body.error;

      if (body.errors) {
        const firstKey = Object.keys(body.errors)[0];
        const firstValue = firstKey ? body.errors[firstKey] : undefined;
        if (Array.isArray(firstValue)) return firstValue[0];
        if (typeof firstValue === "string") return firstValue;
      }
    } catch {
      return undefined;
    }
  }

  try {
    const text = await response.text();
    return text.trim() || undefined;
  } catch {
    return undefined;
  }
}

async function toApiError(
  response: Response,
  fallbackMessage: string
): Promise<ApiError> {
  const detail = await parseErrorDetail(response);
  const message = detail || fallbackMessage;
  return new ApiError(message, response.status, detail);
}

export function getApiErrorMessage(
  error: unknown,
  fallbackMessage: string
): string {
  if (error instanceof ApiError) {
    if (error.detail) return error.detail;
    if (error.status >= 500) return "Erro interno do servidor. Tente novamente.";
    if (error.status === 404) return "Recurso nao encontrado.";
    if (error.status === 409) return "Conflito. Verifique os dados.";
    if (error.status === 400) return "Dados invalidos. Verifique os campos.";
    if (error.status === 401 || error.status === 403) {
      return "Acesso nao autorizado.";
    }
    return error.message || fallbackMessage;
  }

  if (error instanceof Error) {
    if (error.name === "TypeError") {
      return "Falha de conexao com o servidor.";
    }
    return error.message || fallbackMessage;
  }

  return fallbackMessage;
}

//  categoryId para category name
async function mapProductsWithCategories(items: any[], token?: string): Promise<Product[]> {
  try {
    const categoriesResponse = await fetch(`${API_BASE_URL}/api/categories`, {
      headers: getAuthHeaders(token),
    });
    if (!categoriesResponse.ok) {
      throw await toApiError(categoriesResponse, "Falha ao buscar categorias");
    }
    const categoriesData = await categoriesResponse.json();
    const categories = Array.isArray(categoriesData) ? categoriesData : [];
    
    const categoryMap = new Map(categories.map((c: any) => [c.id, c]));
    
    return items.map((item: any) => ({
      ...item,
      quantity: item.stockQuantity,
      category: categoryMap.get(item.categoryId) || undefined,
    }));
  } catch (error) {
    console.error("Erro ao mapear categorias:", error);
    // Fallback: apenas mapear quantity
    return items.map((item: any) => ({
      ...item,
      quantity: item.stockQuantity,
    }));
  }
}

// ----- PRODUTOS -----

export async function fetchProducts(
  filters?: ProductFilters,
  token?: string
): Promise<PaginatedResponse<Product>> {
  try {
    let url = `${API_BASE_URL}/api/products`;
    let response;


    if (filters?.search && filters.search.trim()) {
      response = await fetch(
        `${API_BASE_URL}/api/products/search?name=${encodeURIComponent(filters.search)}`,
        { headers: getAuthHeaders(token) }
      );
      if (!response.ok) {
        throw await toApiError(response, "Falha ao buscar produtos");
      }
      const data = await response.json();
      
      const items = await mapProductsWithCategories(Array.isArray(data) ? data : [], token);
      
      return {
        data: items,
        total: items.length,
        page: 1,
        pageSize: items.length,
        totalPages: 1,
      };
    }

    if (filters?.categoryId && filters.categoryId !== "all") {
      response = await fetch(
        `${API_BASE_URL}/api/products/category/${filters.categoryId}`,
        { headers: getAuthHeaders(token) }
      );
      if (!response.ok) {
        throw await toApiError(
          response,
          "Falha ao buscar produtos por categoria"
        );
      }
      const data = await response.json();
      
      const items = await mapProductsWithCategories(Array.isArray(data) ? data : [], token);
      
      return {
        data: items,
        total: items.length,
        page: 1,
        pageSize: items.length,
        totalPages: 1,
      };
    }

    const params = new URLSearchParams();
    if (filters?.page) params.append("page", filters.page.toString());
    if (filters?.pageSize) params.append("pageSize", filters.pageSize.toString());

    response = await fetch(`${url}?${params}`, {
      headers: getAuthHeaders(token),
    });
    if (!response.ok) {
      throw await toApiError(response, "Falha ao buscar produtos");
    }
    const data = await response.json();

    // Mapear stockQuantity (backend) para quantity (frontend) e adicionar categoria
    const items = await mapProductsWithCategories(data.items || [], token);

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

export async function fetchProductById(id: string, token?: string): Promise<Product | null> {
  try {
    const response = await fetch(`${API_BASE_URL}/api/products/${id}`, {
      headers: getAuthHeaders(token),
    });
    if (response.status === 404) return null;
    if (!response.ok) {
      throw await toApiError(response, "Falha ao buscar produto");
    }
    const data = await response.json();
    
    // Mapear e adicionar categoria
    const [product] = await mapProductsWithCategories([data], token);
    return product || null;
  } catch (error) {
    console.error("Erro ao buscar produto:", error);
    return null;
  }
}

export async function createProduct(data: ProductFormData, token?: string): Promise<Product> {
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
      headers: getAuthHeaders(token),
      body: JSON.stringify(payload),
    });
    if (!response.ok) {
      throw await toApiError(response, "Falha ao criar produto");
    }
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
  data: ProductFormData,
  token?: string
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
      headers: getAuthHeaders(token),
      body: JSON.stringify(payload),
    });
    if (!response.ok) {
      throw await toApiError(response, "Falha ao atualizar produto");
    }
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

export async function deleteProduct(id: string, token?: string): Promise<void> {
  try {
    const response = await fetch(`${API_BASE_URL}/api/products/${id}`, {
      method: "DELETE",
      headers: getAuthHeaders(token),
    });
    if (!response.ok) {
      throw await toApiError(response, "Falha ao deletar produto");
    }
  } catch (error) {
    console.error("Erro ao deletar produto:", error);
    throw error;
  }
}

// ----- CATEGORIAS -----

export async function fetchCategories(token?: string): Promise<Category[]> {
  try {
    const response = await fetch(`${API_BASE_URL}/api/categories`, {
      headers: getAuthHeaders(token),
    });
    if (!response.ok) {
      throw await toApiError(response, "Falha ao buscar categorias");
    }
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

export async function createCategory(data: CategoryFormData, token?: string): Promise<Category> {
  try {
    const response = await fetch(`${API_BASE_URL}/api/categories`, {
      method: "POST",
      headers: getAuthHeaders(token),
      body: JSON.stringify(data),
    });
    if (!response.ok) {
      throw await toApiError(response, "Falha ao criar categoria");
    }
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

export async function updateCategory(
  id: string,
  data: CategoryFormData,
  token?: string
): Promise<Category> {
  try {
    const payload = { id, ...data };
    const response = await fetch(`${API_BASE_URL}/api/categories/${id}`, {
      method: "PUT",
      headers: getAuthHeaders(token),
      body: JSON.stringify(payload),
    });
    if (!response.ok) {
      throw await toApiError(response, "Falha ao atualizar categoria");
    }
    return {
      id,
      ...data,
      createdAt: new Date().toISOString(),
    };
  } catch (error) {
    console.error("Erro ao atualizar categoria:", error);
    throw error;
  }
}

export async function fetchCategoryById(id: string, token?: string): Promise<Category | null> {
  try {
    const categories = await fetchCategories(token);
    return categories.find((c) => c.id === id) || null;
  } catch (error) {
    console.error("Erro ao buscar categoria:", error);
    return null;
  }
}

export async function deleteCategory(id: string, token?: string): Promise<void> {
  try {
    const response = await fetch(`${API_BASE_URL}/api/categories/${id}`, {
      method: "DELETE",
      headers: getAuthHeaders(token),
    });
    if (!response.ok) {
      throw await toApiError(response, "Falha ao deletar categoria");
    }
  } catch (error) {
    console.error("Erro ao deletar categoria:", error);
    throw error;
  }
}

// ----- DASHBOARD -----

export async function fetchDashboardSummary(token?: string) {
  try {
    const response = await fetch(`${API_BASE_URL}/api/dashboard`, {
      headers: getAuthHeaders(token),
    });
    if (!response.ok) {
      throw await toApiError(response, "Falha ao buscar dashboard");
    }
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

export async function fetchDashboardKPIs(token?: string): Promise<DashboardKPIs> {
  const summary = await fetchDashboardSummary(token);
  
  const lowStockAlerts = summary.lowStockProducts?.length || 0;

  return {
    totalProducts: summary.totalProducts || 0,
    totalStockValue: summary.totalStockValue || 0,
    lowStockAlerts,
  };
}

export async function fetchCategoryDistribution(token?: string): Promise<
  CategoryDistribution[]
> {
  const summary = await fetchDashboardSummary(token);
  
  return (summary.productsByCategory || []).map((cat: any, i: number) => ({
    name: cat.categoryName || "Sem categoria",
    value: cat.productCount || 0,
    fill: CHART_COLORS[i % CHART_COLORS.length],
  }));
}

export async function fetchLowStockProducts(token?: string): Promise<Product[]> {
  const summary = await fetchDashboardSummary(token);
  
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

// Export combined dashboard data function
export async function fetchDashboardData(token?: string) {
  const [kpis, categoryDistribution, lowStockProducts] = await Promise.all([
    fetchDashboardKPIs(token),
    fetchCategoryDistribution(token),
    fetchLowStockProducts(token),
  ]);
  
  return {
    kpis,
    categoryDistribution,
    lowStockProducts,
  };
}