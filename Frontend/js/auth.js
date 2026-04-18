function saveToken(token) {
  sessionStorage.setItem("authToken", token);
}

function getToken() {
  return sessionStorage.getItem("authToken");
}

function isLoggedIn() {
  const token = getToken();
  return !!token;
}

function logout() {
  sessionStorage.removeItem("authToken");
}

function requireAuth() {
  const token = getToken();
  if (!token) {
    window.location.href = "index.html";
    return;
  }
  return token;
}

document
  .getElementById("login-form")
  .addEventListener("submit", async function (e) {
    e.preventDefault();
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;
    const button = document.getElementById("login-btn");
    const errorMsg = document.getElementById("error-msg");
    errorMsg.style.display = "none";
    button.disabled = true;
    button.textContent = "Signing in...";

    if (!email || !password) {
      errorMsg.textContent = "Please enter both email and password.";
      errorMsg.style.display = "block";
      button.disabled = false;
      button.textContent = "Sign in";
      return;
    }
    try {
      const data = await login(email, password);
      saveToken(data.token);
      window.location.href = "dashboard.html";
    } catch (error) {
      errorMsg.textContent = error.message;
      button.disabled = false;
      button.textContent = "Sign in";
    }
  });
