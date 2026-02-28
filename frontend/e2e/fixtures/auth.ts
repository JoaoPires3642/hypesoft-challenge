import { test as base } from '@playwright/test';


export const test = base.extend({
  authenticatedPage: async ({ page }, use) => {
    // Navegue para a página de login
    await page.goto('/login');

    // Aguarde o botão de login do Keycloak estar visível
    const keycloakButton = page.getByRole('button', { name: /Entrar com Keycloak/i });
    await keycloakButton.waitFor({ state: 'visible' });

    // Clique para iniciar o fluxo de login
    await keycloakButton.click();

    // Aguarde redirecionamento para Keycloak (pode levar um tempo)
    await page.waitForURL(/keycloak|login/, { timeout: 10000 });

    // Se há campos de login no Keycloak, preencha com as credenciais de teste
    const usernameField = page.getByLabel(/username|email/i).first();
    const passwordField = page.getByLabel(/password/i).first();

    if (await usernameField.isVisible()) {
      await usernameField.fill('testuder');
      await passwordField.fill('testuser');
      await page.getByRole('button', { name: /login|sign in|entrar/i }).click();
    }

    // Aguarde redireção de volta para o dashboard
    await page.waitForURL('/');

    // Aguarde o dashboard carregar (busca por um elemento que só aparece após login)
    await page
      .getByRole('heading', { name: /dashboard|categorias|produtos/i })
      .first()
      .waitFor({ state: 'visible', timeout: 10000 });

    // Use a página autenticada nos testes
    await use(page);
  },
});

export { expect } from '@playwright/test';
