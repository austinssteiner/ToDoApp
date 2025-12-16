import { useState, useEffect } from 'react';
import { api } from '../services/api';
import TaskForm from './TaskForm';
import TaskDetail from './TaskDetail';
import ConfirmModal from './ConfirmModal';

export default function TasksList({ user }) {
  const [tasks, setTasks] = useState([]);
  const [showTaskForm, setShowTaskForm] = useState(false);
  const [selectedTask, setSelectedTask] = useState(null);
  const [showDeleteTaskModal, setShowDeleteTaskModal] = useState(false);
  const [taskToDelete, setTaskToDelete] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // For now, we'll store tasks in localStorage since we don't have a GET endpoint
  // In a real app, you'd fetch tasks from the API
  useEffect(() => {
    const savedTasks = localStorage.getItem(`tasks_${user.userId}`);
    if (savedTasks) {
      setTasks(JSON.parse(savedTasks));
    }
  }, [user.userId]);

  const saveTasksToStorage = (newTasks) => {
    setTasks(newTasks);
    localStorage.setItem(`tasks_${user.userId}`, JSON.stringify(newTasks));
  };

  const handleCreateTask = async (taskData) => {
    setError('');
    setLoading(true);
    try {
      const newTask = await api.createTask({
        userId: user.userId,
        taskName: taskData.taskName,
        description: taskData.description,
        createdBy: user.userId,
      });
      
      const updatedTasks = [...tasks, newTask];
      saveTasksToStorage(updatedTasks);
      setShowTaskForm(false);
    } catch (err) {
      setError(err.message || 'Failed to create task');
    } finally {
      setLoading(false);
    }
  };

  const handleTaskClick = (task) => {
    setSelectedTask(task);
  };

  const handleBackToList = () => {
    setSelectedTask(null);
  };

  const handleTaskUpdate = (updatedTask) => {
    const updatedTasks = tasks.map(t => 
      t.taskId === updatedTask.taskId ? updatedTask : t
    );
    saveTasksToStorage(updatedTasks);
    setSelectedTask(updatedTask);
  };

  const handleTaskDeleteClick = (taskId) => {
    setTaskToDelete(taskId);
    setShowDeleteTaskModal(true);
  };

  const handleConfirmDeleteTask = async () => {
    if (!taskToDelete) return;

    setError('');
    setLoading(true);
    try {
      await api.deleteTask(taskToDelete);
      const updatedTasks = tasks.filter(t => t.taskId !== taskToDelete);
      saveTasksToStorage(updatedTasks);
      if (selectedTask?.taskId === taskToDelete) {
        setSelectedTask(null);
      }
      setShowDeleteTaskModal(false);
      setTaskToDelete(null);
    } catch (err) {
      setError(err.message || 'Failed to delete task');
    } finally {
      setLoading(false);
    }
  };

  if (selectedTask) {
    return (
      <TaskDetail
        task={selectedTask}
        user={user}
        onBack={handleBackToList}
        onTaskUpdate={handleTaskUpdate}
        onTaskDelete={handleTaskDeleteClick}
      />
    );
  }

  return (
    <div className="tasks-container">
      <div className="tasks-header">
        <h1>My Tasks</h1>
        <button 
          onClick={() => setShowTaskForm(true)} 
          className="btn-primary"
        >
          + New Task
        </button>
      </div>

      {error && <div className="error-message">{error}</div>}

      {showTaskForm && (
        <TaskForm
          onSubmit={handleCreateTask}
          onCancel={() => setShowTaskForm(false)}
          loading={loading}
        />
      )}

      <div className="tasks-list">
        {tasks.length === 0 ? (
          <div className="empty-state">
            <p>No tasks yet. Create your first task to get started!</p>
          </div>
        ) : (
          tasks.map((task) => (
            <div
              key={task.taskId}
              className="task-card"
              onClick={() => handleTaskClick(task)}
            >
              <div className="task-header">
                <h3>{task.taskName}</h3>
                {task.completedDate && (
                  <span className="task-completed">✓ Completed</span>
                )}
              </div>
              {task.description && (
                <p className="task-description">{task.description}</p>
              )}
              <div className="task-footer">
                <span className="task-date">
                  Created: {new Date(task.createdDate).toLocaleDateString()}
                </span>
              </div>
            </div>
          ))
        )}
      </div>

      <ConfirmModal
        isOpen={showDeleteTaskModal}
        title="Delete Task"
        message={taskToDelete ? `Are you sure you want to delete "${tasks.find(t => t.taskId === taskToDelete)?.taskName || 'this task'}"? This will also delete all associated subtasks. This action cannot be undone.` : ''}
        confirmText="Delete Task"
        onConfirm={handleConfirmDeleteTask}
        onCancel={() => {
          setShowDeleteTaskModal(false);
          setTaskToDelete(null);
        }}
        loading={loading}
      />
    </div>
  );
}

