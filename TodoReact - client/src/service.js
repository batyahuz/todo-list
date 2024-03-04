import axios from 'axios';

axios.defaults.baseURL = "https://localhost:7283";
setAuthorizationBearer()

axios.interceptors.response.use(
  function (response) { return response },
  function (error) {
    if (error.response?.status === 401)
      return (window.location.href = '/login');
    return Promise.reject(error)
  }
);

function saveAccessToken(authresult) {
  localStorage.setItem("accessToken", authresult.token)
  setAuthorizationBearer()
}

function setAuthorizationBearer() {
  const accessToken = localStorage.getItem("accessToken")
  axios.defaults.headers.common["Authorization"] = accessToken ? `Bearer ${accessToken}` : '';
}

const taskService = {
  getTasks: async () => {
    const result = await axios.get(`/items`)
    return result.data;
  },

  addTask: async (name) => {
    const result = await axios.post(`/items`, { id: 0, name: name, isComplete: false })
    return result.data;
  },

  setCompleted: async (id, isComplete) => {
    const result = await axios.put(`/items/${id}`, { id, isComplete })
    return result.data;
  },

  deleteTask: async (id) => {
    const result = await axios.delete(`/items/${id}`)
    return result.data
  },

  login: async (user) => {
    const result = await axios.post(`/login`, { ...user })
    saveAccessToken(result.data)
    return result.data;
  },

  signin: async (user) => {
    const result = await axios.post(`/signin`, { ...user })
    saveAccessToken(result.data)
    return result.data;
  },

  connected: () => {
    const storage = localStorage.getItem("accessToken")
    return storage !== null && storage.length !== 0;
  },

  signout: () => {
    localStorage.removeItem("accessToken")
    setAuthorizationBearer()
  }

};

export default taskService
