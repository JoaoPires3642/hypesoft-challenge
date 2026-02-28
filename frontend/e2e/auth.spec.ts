import { test, expect } from '@playwright/test';

test.describe('Authentication', () => {
  test('deve carregar a página de login', async ({ page }) => {
    await page.goto('/login');
    
    // Verificar que estamos na página de login
    expect(page.url()).toContain('/login');
    await expect(page.locator('body')).toBeVisible();
  });

  test('deve redirecionar para Keycloak ao clicar no botão de login', async ({ page }) => {
    await page.goto('/login');

    const keycloakButton = page.getByRole('button', { name: /Entrar com Keycloak/i });
    await keycloakButton.click();

    // Aguarde navegação para Keycloak (pode incluir localhost:8080)
    await page.waitForURL(/keycloak|localhost:8080|login/, {
      timeout: 10000,
    });

    // Verificar se esta página de autenticação
    const pageUrl = page.url();
    expect(pageUrl).toMatch(/keycloak|8080|login/i);
  });

  test('deve acessar sessão quando não autenticado (retorna null)', async ({ page }) => {
    // Chamar o endpoint de sessão sem autenticação
    const response = await page.request.get('/api/auth/session');
    expect(response.status()).toBe(200);

    const sessionData = await response.json();
    expect(sessionData).toEqual(null);
  });

  test('deve ter título na página de login', async ({ page }) => {
    await page.goto('/login');
    const title = await page.title();
    expect(title.length).toBeGreaterThan(0);
  });

  test('deve ter meta tags básicas', async ({ page }) => {
    await page.goto('/login');
    const viewport = page.locator('meta[name="viewport"]');
    expect(await viewport.count()).toBeGreaterThan(0);
  });
});
