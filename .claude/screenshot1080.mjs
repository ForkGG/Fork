import { chromium } from 'playwright';
import path from 'path';
import { fileURLToPath } from 'url';
const __dirname = path.dirname(fileURLToPath(import.meta.url));
const OUT = path.join(__dirname, 'screenshots');

(async () => {
  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage({ viewport: { width: 1920, height: 1080 } });
  page.on('console', msg => { if (msg.type() === 'error') console.error('[browser]', msg.text()); });

  await page.goto('http://localhost:5199', { waitUntil: 'networkidle', timeout: 30000 });
  await page.waitForTimeout(4000);

  // Click Settings tab gear icon
  await page.mouse.click(281, 131);
  await page.waitForTimeout(3000);

  // Wait for the settings sidebar to appear
  await page.waitForSelector('text=server.properties', { timeout: 20000 });
  await page.locator('text=server.properties').first().click();
  await page.waitForTimeout(1000);

  await page.screenshot({ path: `${OUT}/1080-top.png` });

  const pane = page.locator('.overflow-y-auto').first();
  if (await pane.count() > 0) {
    await pane.evaluate(el => el.scrollTop = 600);
    await page.waitForTimeout(200);
    await page.screenshot({ path: `${OUT}/1080-mid.png` });
    await pane.evaluate(el => el.scrollTop = el.scrollHeight);
    await page.waitForTimeout(200);
    await page.screenshot({ path: `${OUT}/1080-bottom.png` });
  }

  await browser.close();
  console.log('done');
})();
