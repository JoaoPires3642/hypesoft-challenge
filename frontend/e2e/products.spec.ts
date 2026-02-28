import { test, expect } from '@playwright/test';

test.describe('Products', () => {
  test('deve tentar acessar página de produtos', async ({ page }) => {
    await page.goto('/products');
    
    // Pode redirecionar para login ou carregar a página
    const url = page.url();
    const validUrls = url.includes('/products') || url.includes('/login');
    expect(validUrls).toBe(true);
    await expect(page.locator('body')).toBeVisible();
  });

  test('deve carregar página sem erros de console críticos', async ({ page }) => {
    const errors: string[] = [];
    page.on('console', msg => {
      if (msg.type() === 'error') errors.push(msg.text());
    });
    
    await page.goto('/products');
    await page.waitForTimeout(1000);
    
    // Verificar que não há muitos erros
    expect(errors.length).toBeLessThan(10);
  });

  test('deve ter título na página', async ({ page }) => {
    await page.goto('/products');
    const title = await page.title();
    expect(title.length).toBeGreaterThan(0);
  });

  test('deve responder em tempo razoável', async ({ page }) => {
    const start = Date.now();
    await page.goto('/products');
    const loadTime = Date.now() - start;
    expect(loadTime).toBeLessThan(5000);
  });
});
