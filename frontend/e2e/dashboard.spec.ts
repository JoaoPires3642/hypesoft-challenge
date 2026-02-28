import { test, expect } from '@playwright/test';

test.describe('Dashboard', () => {
  test('deve carregar a página inicial', async ({ page }) => {
    await page.goto('/');
    expect(page.url()).toMatch(/localhost:3000/);
    await expect(page.locator('body')).toBeVisible();
  });

  test('deve ter navegação visível', async ({ page }) => {
    await page.goto('/');
    
    // Verificar se há links de navegação
    const links = page.locator('a[href*="/"]');
    const count = await links.count();
    expect(count).toBeGreaterThan(0);
  });

  test('deve tentar navegar para outras páginas', async ({ page }) => {
    await page.goto('/categories');
    await expect(page.locator('body')).toBeVisible();
    
    await page.goto('/products');
    await expect(page.locator('body')).toBeVisible();
  });

  test('deve ter título na página inicial', async ({ page }) => {
    await page.goto('/');
    const title = await page.title();
    expect(title.length).toBeGreaterThan(0);
  });

  test('deve carregar em tempo razoável', async ({ page }) => {
    const start = Date.now();
    await page.goto('/');
    const loadTime = Date.now() - start;
    expect(loadTime).toBeLessThan(5000);
  });
});
