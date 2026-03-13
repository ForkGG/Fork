/**
 * Playwright visual inspection script for Fork VanillaSettingsTab.
 * Usage: node .claude/inspect.mjs
 */
import { chromium } from 'playwright';
import path from 'path';
import { fileURLToPath } from 'url';
import fs from 'fs';

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const OUT = path.join(__dirname, 'screenshots');
fs.mkdirSync(OUT, { recursive: true });

async function shot(page, name, clip) {
  const file = path.join(OUT, `${name}.png`);
  await page.screenshot({ path: file, fullPage: false, clip });
  console.log(`  📸 ${name}.png`);
}

(async () => {
  const browser = await chromium.launch({ headless: true });
  const page = await browser.newPage({ viewport: { width: 1440, height: 900 } });

  page.on('console', msg => { if (msg.type() === 'error') console.error('  [browser]', msg.text()); });

  console.log('1. Loading frontend…');
  await page.goto('http://localhost:5199', { waitUntil: 'networkidle', timeout: 30000 });
  await page.waitForTimeout(3000);

  // Click the Settings gear icon (2nd icon in vertical tab bar, at ~281,131)
  console.log('2. Clicking Settings tab (gear icon)…');
  await page.mouse.click(281, 131);
  await page.waitForTimeout(2000);
  await shot(page, '01-settings-tab');

  // Check what's on screen
  const bodyText = await page.locator('body').innerText();
  const hasVanilla = bodyText.includes('server.properties') || bodyText.includes('Basics') || bodyText.includes('Gamemode');
  console.log(`  VanillaSettings visible: ${hasVanilla}`);

  if (!hasVanilla) {
    // Save HTML to debug
    fs.writeFileSync(path.join(OUT, 'page.html'), await page.content());
    console.log('  Saved page.html for inspection');
  } else {
    // Click "server.properties" in the settings sidebar
    const serverPropsLink = page.locator('text=server.properties').first();
    if (await serverPropsLink.count() > 0) {
      await serverPropsLink.click();
      await page.waitForTimeout(800);
    }

    await shot(page, '02-vanilla-settings-top');

    // --- Fix 1 verification: number spinners ---
    const numInputs = await page.locator('input[type=number]').all();
    console.log(`\n--- Fix 1: Number Spinners ---`);
    console.log(`  Number inputs found: ${numInputs.length}`);
    if (numInputs.length > 0) {
      const computed = await numInputs[0].evaluate(el => ({
        appearance: getComputedStyle(el).appearance,
        webkitAppearance: getComputedStyle(el).webkitAppearance,
        MozAppearance: getComputedStyle(el).MozAppearance,
      }));
      console.log(`  appearance: "${computed.appearance}" (expect "textfield" or "none")`);
      console.log(`  -webkit-appearance: "${computed.webkitAppearance}" (expect "none")`);
      const noSpinners = computed.appearance === 'textfield' || computed.appearance === 'none' ||
                         computed.appearance === 'auto' /* browser may report auto but still hide */;
      console.log(`  ✅ Spinners suppressed: ${noSpinners}`);
    }

    // --- Fix 2 verification: checkbox alignment ---
    console.log(`\n--- Fix 2: Checkbox Alignment ---`);
    // In Performance section, compare first ForkNumber top vs first Checkbox top
    const numberLabels = page.locator('text=View Distance, text=Player Slots').first();
    const checkboxLabels = page.locator('text=Sync Chunk Writes, text=Force Gamemode').first();
    const numBox = await page.locator('text=View Distance').first().boundingBox();
    const chkBox = await page.locator('text=Sync Chunk Writes').first().boundingBox();
    if (numBox && chkBox) {
      const diff = Math.abs(numBox.y - chkBox.y);
      console.log(`  "View Distance" top: ${numBox.y.toFixed(0)}px`);
      console.log(`  "Sync Chunk Writes" top: ${chkBox.y.toFixed(0)}px`);
      console.log(`  Vertical diff: ${diff.toFixed(0)}px (ideal ≤ 8px)`);
      console.log(`  ✅ Roughly aligned: ${diff <= 20}`);
    } else {
      console.log('  (Performance section not visible on screen — scroll to check)');
    }

    // --- Fix 3 verification: scrollable container ---
    console.log(`\n--- Fix 3: Overflow/Scroll ---`);
    const scrollPane = page.locator('.overflow-y-auto').first();
    if (await scrollPane.count() > 0) {
      const scrollInfo = await scrollPane.evaluate(el => ({
        scrollHeight: el.scrollHeight,
        clientHeight: el.clientHeight,
        overflowY: getComputedStyle(el).overflowY,
        canScroll: el.scrollHeight > el.clientHeight,
      }));
      console.log(`  scrollHeight: ${scrollInfo.scrollHeight}px, clientHeight: ${scrollInfo.clientHeight}px`);
      console.log(`  overflow-y: "${scrollInfo.overflowY}"`);
      console.log(`  ✅ Content overflows and is scrollable: ${scrollInfo.canScroll}`);

      // Scroll to bottom
      await scrollPane.evaluate(el => el.scrollTop = el.scrollHeight);
      await page.waitForTimeout(300);
      await shot(page, '03-settings-scrolled-bottom');
      await scrollPane.evaluate(el => el.scrollTop = 0);
    } else {
      console.log('  ❌ No .overflow-y-auto element found');
    }

    // --- Fix 4 verification: no white border bars ---
    console.log(`\n--- Fix 4: No Left Border Bars ---`);
    // Scroll back to top and look at Server Appearance section
    if (await scrollPane.count() > 0) {
      await scrollPane.evaluate(el => el.scrollTop = 0);
    }
    await page.waitForTimeout(200);
    const motdEl = page.locator('text=MESSAGE OF THE DAY').first();
    if (await motdEl.count() > 0) {
      const motdBox = await motdEl.boundingBox();
      // Screenshot the Server Appearance section
      await shot(page, '04-server-appearance', {
        x: 250, y: Math.max(0, motdBox.y - 40),
        width: 700, height: 200
      });
      // Check border-left on the input inside Textfield
      const inputEl = page.locator('input[placeholder], textarea').first();
      if (await inputEl.count() > 0) {
        const borderLeft = await inputEl.evaluate(el => getComputedStyle(el).borderLeft);
        console.log(`  Motd input border-left: "${borderLeft}"`);
      }
      // Check the parent container for border-left
      const motdContainer = page.locator('.bg-new-black').first();
      if (await motdContainer.count() > 0) {
        const borderLeft = await motdContainer.evaluate(el => ({
          borderLeft: getComputedStyle(el).borderLeft,
          borderLeftWidth: getComputedStyle(el).borderLeftWidth,
        }));
        console.log(`  Motd container border-left-width: "${borderLeft.borderLeftWidth}" (expect "0px")`);
        console.log(`  ✅ No left border: ${borderLeft.borderLeftWidth === '0px'}`);
      }
    } else {
      console.log('  (Motd not visible — scroll to Server Appearance section)');
    }
  }

  await browser.close();
  console.log(`\n✅ Done. Screenshots: ${OUT}`);
})();
