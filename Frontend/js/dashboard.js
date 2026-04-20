async function loadDashboard() {
  const token = requireAuth();
  if (!token) return;
  const loadingMsg = document.getElementById("loading-msg");
  const errorMsg = document.getElementById("error-msg");
  const accountsContainer = document.getElementById("accounts-container");

  try {
    const accounts = await getAccounts(token);
    loadingMsg.style.display = "none";
    accountsContainer.innerHTML = "";

    accounts.forEach((account) => {
      const card = document.createElement("div");
      card.className = "account-card";
      card.innerHTML = `
        <h2>${account.type}</h2>
        <p>${account.accountNumber}</p>
        <p>$${account.balance.toFixed(2)}</p>
      `;
      accountsContainer.appendChild(card);
    });

    const hasChequing = accounts.some((a) => a.type === "Chequing");
    const hasSavings = accounts.some((a) => a.type === "Savings");

    if (hasChequing && hasSavings) {
      document.getElementById("create-account-section").style.display = "none";
    } else {
      document.getElementById("create-account-section").style.display = "block";
    }
  } catch (error) {
    loadingMsg.style.display = "none";
    errorMsg.textContent = error.message;
    errorMsg.style.display = "block";
  }
}

document
  .getElementById("create-account-btn")
  .addEventListener("click", async function () {
    const token = requireAuth();
    if (!token) return;

    const accountType = parseInt(
      document.getElementById("account-type-select").value,
    );
    const errorMsg = document.getElementById("create-error-msg");
    const successMsg = document.getElementById("create-success-msg");
    const btn = document.getElementById("create-account-btn");

    errorMsg.style.display = "none";
    successMsg.style.display = "none";

    btn.disabled = true;
    btn.textContent = "Creating...";

    try {
      await createAccount(token, accountType);
      successMsg.textContent = "Account created successfully!";
      successMsg.style.display = "block";
      await loadDashboard();
    } catch (error) {
      errorMsg.textContent = error.message;
      errorMsg.style.display = "block";
    } finally {
      btn.disabled = false;
      btn.textContent = "Create Account";
    }
  });

loadDashboard();
