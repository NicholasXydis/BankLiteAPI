async function loadDeposit() {
  const token = requireAuth();
  if (!token) return;

  const accountSelect = document.getElementById("account-select");
  const balanceDisplay = document.getElementById("balance-display");
  const errorMsg = document.getElementById("error-msg");
  const successMsg = document.getElementById("success-msg");
  let accounts = [];

  try {
    accounts = await getAccounts(token);

    if (accounts.length === 0) {
      document.getElementById("empty-state").style.display = "block";
      document.querySelector(".form-card").style.display = "none";
      return;
    }
    document.querySelector(".form-card").style.display = "block";

    accountSelect.innerHTML = "";
    accounts.forEach((account) => {
      const option = document.createElement("option");
      option.value = account.id;
      option.textContent = `${account.type} - $${account.balance.toFixed(2)}`;
      accountSelect.appendChild(option);
    });

    balanceDisplay.textContent = `$${accounts[0].balance.toFixed(2)}`;

    accountSelect.addEventListener("change", function () {
      const selected = accounts.find((a) => a.id === this.value);
      if (selected) {
        balanceDisplay.textContent = `$${selected.balance.toFixed(2)}`;
      }
    });
  } catch (error) {
    errorMsg.textContent = error.message;
    errorMsg.style.display = "block";
  }
}

document
  .getElementById("deposit-btn")
  .addEventListener("click", async function () {
    const token = requireAuth();
    if (!token) return;

    const errorMsg = document.getElementById("error-msg");
    const successMsg = document.getElementById("success-msg");
    const accountId = document.getElementById("account-select").value;
    const amount = parseFloat(document.getElementById("amount").value);

    errorMsg.style.display = "none";
    successMsg.style.display = "none";

    if (!accountId) {
      errorMsg.textContent = "Please select an account.";
      errorMsg.style.display = "block";
      return;
    }

    if (!amount || amount <= 0) {
      errorMsg.textContent = "Please enter a valid amount.";
      errorMsg.style.display = "block";
      return;
    }

    const btn = document.getElementById("deposit-btn");
    btn.disabled = true;
    btn.classList.add("btn-loading");

    try {
      await deposit(token, accountId, amount);
      successMsg.textContent = `Successfully deposited $${amount.toFixed(2)}!`;
      successMsg.style.display = "block";
      document.getElementById("amount").value = "";
      await loadDeposit();
    } catch (error) {
      errorMsg.textContent = error.message;
      errorMsg.style.display = "block";
    } finally {
      btn.disabled = false;
      btn.classList.remove("btn-loading");
    }
  });
loadDeposit();
