import { useState, useEffect } from 'react';
import { api } from '../services/api';
import TaskForm from './TaskForm';
import ConfirmModal from './ConfirmModal';

export default function TaskDetail({ task, user, onBack, onTaskUpdate, onTaskDelete }) {
  const [subtasks, setSubtasks] = useState([]);
  const [showSubtaskForm, setShowSubtaskForm] = useState(false);
  const [showTaskForm, setShowTaskForm] = useState(false);
  const [showDeleteTaskModal, setShowDeleteTaskModal] = useState(false);
  const [showDeleteSubtaskModal, setShowDeleteSubtaskModal] = useState(false);
  const [subtaskToDelete, setSubtaskToDelete] = useState(null);
  const [subtaskDescription, setSubtaskDescription] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // Load subtasks from localStorage
  useEffect(() => {
    const savedSubtasks = localStorage.getItem(`subtasks_${task.taskId}`);
    if (savedSubtasks) {
      setSubtasks(JSON.parse(savedSubtasks));
    }
  }, [task.taskId]);

  const saveSubtasksToStorage = (newSubtasks) => {
    setSubtasks(newSubtasks);
    localStorage.setItem(`subtasks_${task.taskId}`, JSON.stringify(newSubtasks));
  };

  const handleCreateSubtask = async (e) => {
    e.preventDefault();
    if (!subtaskDescription.trim()) return;

    setError('');
    setLoading(true);
    try {
      const newSubtask = await api.createSubtask({
        taskId: task.taskId,
        description: subtaskDescription.trim(),
        createdBy: user.userId,
      });

      const updatedSubtasks = [...subtasks, newSubtask];
      saveSubtasksToStorage(updatedSubtasks);
      setSubtaskDescription('');
      setShowSubtaskForm(false);
    } catch (err) {
      setError(err.message || 'Failed to create subtask');
    } finally {
      setLoading(false);
    }
  };

  const handleToggleSubtask = async (subtask) => {
    setError('');
    setLoading(true);
    try {
      const updatedSubtask = await api.updateSubtask(subtask.subtaskId, {
        completedDate: subtask.completedDate ? null : new Date().toISOString(),
      });

      const updatedSubtasks = subtasks.map(s =>
        s.subtaskId === subtask.subtaskId ? updatedSubtask : s
      );
      saveSubtasksToStorage(updatedSubtasks);
    } catch (err) {
      setError(err.message || 'Failed to update subtask');
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteSubtaskClick = (subtaskId) => {
    setSubtaskToDelete(subtaskId);
    setShowDeleteSubtaskModal(true);
  };

  const handleConfirmDeleteSubtask = async () => {
    if (!subtaskToDelete) return;

    setError('');
    setLoading(true);
    try {
      await api.deleteSubtask(subtaskToDelete);
      const updatedSubtasks = subtasks.filter(s => s.subtaskId !== subtaskToDelete);
      saveSubtasksToStorage(updatedSubtasks);
      setShowDeleteSubtaskModal(false);
      setSubtaskToDelete(null);
    } catch (err) {
      setError(err.message || 'Failed to delete subtask');
    } finally {
      setLoading(false);
    }
  };

  const handleTaskUpdate = async (taskData) => {
    setError('');
    setLoading(true);
    try {
      const updatedTask = await api.updateTask(task.taskId, taskData);
      onTaskUpdate(updatedTask);
      setShowTaskForm(false);
    } catch (err) {
      setError(err.message || 'Failed to update task');
    } finally {
      setLoading(false);
    }
  };

  const handleTaskComplete = async () => {
    setError('');
    setLoading(true);
    try {
      const updatedTask = await api.updateTask(task.taskId, {
        completedDate: task.completedDate ? null : new Date().toISOString(),
      });
      onTaskUpdate(updatedTask);
    } catch (err) {
      setError(err.message || 'Failed to update task');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="task-detail-container">
      <div className="task-detail-header">
        <button onClick={onBack} className="btn-back">← Back</button>
        <div className="task-detail-actions">
          <button onClick={handleTaskComplete} className="btn-secondary">
            {task.completedDate ? 'Mark Incomplete' : 'Mark Complete'}
          </button>
          <button onClick={() => setShowTaskForm(true)} className="btn-secondary">
            Edit Task
          </button>
          <button onClick={() => setShowDeleteTaskModal(true)} className="btn-danger">
            Delete Task
          </button>
        </div>
      </div>

      {showTaskForm && (
        <TaskForm
          initialTask={task}
          onSubmit={handleTaskUpdate}
          onCancel={() => setShowTaskForm(false)}
          loading={loading}
        />
      )}

      <div className="task-detail-content">
        <div className="task-detail-title">
          <h1>{task.taskName}</h1>
          {task.completedDate && (
            <span className="task-completed-badge">✓ Completed</span>
          )}
        </div>
        {task.description && (
          <p className="task-detail-description">{task.description}</p>
        )}

        <div className="subtasks-section">
          <div className="subtasks-header">
            <h2>Subtasks</h2>
            {!showSubtaskForm && (
              <button
                onClick={() => setShowSubtaskForm(true)}
                className="btn-primary btn-small"
              >
                + Add Subtask
              </button>
            )}
          </div>

          {showSubtaskForm && (
            <form onSubmit={handleCreateSubtask} className="subtask-form">
              <input
                type="text"
                value={subtaskDescription}
                onChange={(e) => setSubtaskDescription(e.target.value)}
                placeholder="Enter subtask description"
                autoFocus
              />
              <div className="subtask-form-actions">
                <button
                  type="button"
                  onClick={() => {
                    setShowSubtaskForm(false);
                    setSubtaskDescription('');
                  }}
                  className="btn-secondary btn-small"
                >
                  Cancel
                </button>
                <button type="submit" disabled={loading} className="btn-primary btn-small">
                  {loading ? 'Adding...' : 'Add'}
                </button>
              </div>
            </form>
          )}

          {error && <div className="error-message">{error}</div>}

          <div className="subtasks-list">
            {subtasks.length === 0 ? (
              <p className="empty-state">No subtasks yet. Add one to get started!</p>
            ) : (
              subtasks.map((subtask) => (
                <div
                  key={subtask.subtaskId}
                  className={`subtask-item ${subtask.completedDate ? 'completed' : ''}`}
                >
                  <label className="subtask-checkbox">
                    <input
                      type="checkbox"
                      checked={!!subtask.completedDate}
                      onChange={() => handleToggleSubtask(subtask)}
                      disabled={loading}
                    />
                    <span>{subtask.description}</span>
                  </label>
                  <button
                    onClick={() => handleDeleteSubtaskClick(subtask.subtaskId)}
                    className="btn-icon"
                    disabled={loading}
                  >
                    ×
                  </button>
                </div>
              ))
            )}
          </div>
        </div>
      </div>

      <ConfirmModal
        isOpen={showDeleteTaskModal}
        title="Delete Task"
        message={`Are you sure you want to delete "${task.taskName}"? This will also delete all associated subtasks. This action cannot be undone.`}
        confirmText="Delete Task"
        onConfirm={() => {
          setShowDeleteTaskModal(false);
          onTaskDelete(task.taskId);
        }}
        onCancel={() => setShowDeleteTaskModal(false)}
        loading={loading}
      />

      <ConfirmModal
        isOpen={showDeleteSubtaskModal}
        title="Delete Subtask"
        message="Are you sure you want to delete this subtask? This action cannot be undone."
        confirmText="Delete Subtask"
        onConfirm={handleConfirmDeleteSubtask}
        onCancel={() => {
          setShowDeleteSubtaskModal(false);
          setSubtaskToDelete(null);
        }}
        loading={loading}
      />
    </div>
  );
}

