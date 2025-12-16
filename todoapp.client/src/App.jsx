import { useState } from 'react';
import SignIn from './components/SignIn';
import TasksList from './components/TasksList';
import './App.css';

function App() {
  const [user, setUser] = useState(null);

  const handleSignIn = (userData) => {
    setUser(userData);
  };

  const handleSignOut = () => {
    setUser(null);
  };

  if (!user) {
    return <SignIn onSignIn={handleSignIn} />;
  }

  return (
    <div className="app">
      <header className="app-header">
        <div className="app-header-content">
          <h1>ToDo App</h1>
          <div className="user-info">
            <span>Welcome, {user.firstName} {user.lastName}</span>
            <button onClick={handleSignOut} className="btn-secondary btn-small">
              Sign Out
            </button>
          </div>
        </div>
      </header>
      <main className="app-main">
        <TasksList user={user} />
      </main>
    </div>
  );
}

export default App;
