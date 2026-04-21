async function loadTransfer() {
  const token = requireAuth();
  if (!token) return;

  const accountSelect = document.getElementById("account-select");
  const toAccountSelect = document.getElementById("to-account-select");
  const errorMsg = document.getElementById("error-msg");
  const successMsg = document.getElementById("success-msg");
  let accounts = [];

  try {
    accounts = await getAccounts(token);
    if (accounts.length === 0) {
      document.getElementById("loading-msg").style.display = "none";
      document.getElementById("empty-state").style.display = "block";
      document.querySelector(".form-card").style.display = "none";
      return;
    }
    document.getElementById("loading-msg").style.display = "none";
    document.querySelector(".form-card").style.display = "block";

    accountSelect.innerHTML = "";
    accounts.forEach((account) => {
      const option = document.createElement("option");
      option.value = account.id;
      option.textContent = `${account.type} - $${account.balance.toFixed(2)}`;
      accountSelect.appendChild(option);
    });

    toAccountSelect.innerHTML = "";
    accounts.forEach((account) => {
      const option = document.createElement("option");
      option.value = account.id;
      option.textContent = `${account.type} - $${account.balance.toFixed(2)}`;
      toAccountSelect.appendChild(option);
    });
  } catch (error) {
    errorMsg.textContent = error.message;
    errorMsg.style.display = "block";
  }
}

document
  .getElementById("transfer-btn")
  .addEventListener("click", async function () {
    const token = requireAuth();
    if (!token) return;

    const errorMsg = document.getElementById("error-msg");
    const successMsg = document.getElementById("success-msg");
    const accountId = document.getElementById("account-select").value;
    const toAccountId = document.getElementById("to-account-select").value;
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

    if (toAccountId === accountId) {
      errorMsg.textContent = "Cannot transfer to same account";
      errorMsg.style.display = "block";
      return;
    }

    const btn = document.getElementById("transfer-btn");
    btn.disabled = true;
    btn.textContent = "Transferring...";

    try {
      await transfer(token, accountId, toAccountId, amount);
      successMsg.textContent = `Successfully transferred $${amount.toFixed(2)}!`;
      successMsg.style.display = "block";
      document.getElementById("amount").value = "";
      await loadTransfer();
    } catch (error) {
      errorMsg.textContent = error.message;
      errorMsg.style.display = "block";
    } finally {
      btn.disabled = false;
      btn.textContent = "Transfer";
    }
  });
loadTransfer();
