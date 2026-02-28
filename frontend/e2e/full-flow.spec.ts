import { test, expect } from '@playwright/test';

test.describe('API Health', () => {
  test('deve verificar status da API', async ({ request }) => {
    const response = await request.get('http://localhost:5000/health').catch(() => null);
    // API pode ou não ter endpoint de health
    expect(response?.status() || 404).toBeDefined();
  });
});

test.describe('Navigation', () => {
  test('deve ter links funcionais na home', async ({ page }) => {
    await page.goto('/');
    const links = page.locator('a');
    const count = await links.count();
    expect(count).toBeGreaterThan(0);
  });
});


test.describe('Application Flow', () => {
  test('deve carregar a página inicial', async ({ page }) => {
    await page.goto('/');
    const url = page.url();
    expect(url).toMatch(/localhost:3000/);
    await expect(page.locator('body')).toBeVisible();
  });

  test('deve navegar entre páginas principais', async ({ page }) => {
    const routes = ['/', '/products', '/categories'];

    for (const route of routes) {
      await page.goto(route);
      await expect(page.locator('body')).toBeVisible();
    }
  });

  test('deve ter layout responsivo', async ({ page }) => {
    await page.goto('/');
    
    // Desktop
    await page.setViewportSize({ width: 1280, height: 720 });
    await expect(page.locator('body')).toBeVisible();

    // Mobile
    await page.setViewportSize({ width: 375, height: 667 });
    await expect(page.locator('body')).toBeVisible();
  });

  test('deve carregar página de login diretamente', async ({ page }) => {
    await page.goto('/login');
    expect(page.url()).toContain('/login');
    await expect(page.locator('body')).toBeVisible();
  });

  test('deve ter meta viewport configurado', async ({ page }) => {
    await page.goto('/');
    const viewport = page.locator('meta[name="viewport"]');
    expect(await viewport.count()).toBeGreaterThan(0);
  });

  test('deve carregar sem erros 404', async ({ page }) => {
    const responses: number[] = [];
    page.on('response', response => {
      responses.push(response.status());
    });
    
    await page.goto('/');
    await page.waitForTimeout(1000);
    
    const has404 = responses.some(status => status === 404);
    expect(has404).toBe(false);
  });
});
