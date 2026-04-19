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
  } catch (error) {
    loadingMsg.style.display = "none";
    errorMsg.textContent = error.message;
    errorMsg.style.display = "block";
  }
}

document.getElementById("logout-btn").addEventListener("click", function () {
  logout();
  window.location.href = "index.html";
});

loadDashboard();
