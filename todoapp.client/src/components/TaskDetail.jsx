import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../services/api';
import TaskForm from './TaskForm';
import ConfirmModal from './ConfirmModal';

export default function TaskDetail({ task, user, onBack, onTaskUpdate, onTaskDelete }) {
  const [showSubtaskForm, setShowSubtaskForm] = useState(false);
  const [showTaskForm, setShowTaskForm] = useState(false);
  const [showDeleteTaskModal, setShowDeleteTaskModal] = useState(false);
  const [showDeleteSubtaskModal, setShowDeleteSubtaskModal] = useState(false);
  const [subtaskToDelete, setSubtaskToDelete] = useState(null);
  const [subtaskDescription, setSubtaskDescription] = useState('');
  const queryClient = useQueryClient();

  // Fetch task with subtasks using React Query
  const { data: taskData, error, isLoading: isLoadingTask } = useQuery({
    queryKey: ['task', task.taskId],
    queryFn: () => api.getTask(task.taskId),
    initialData: task, // Use the passed task as initial data
  });

  const subtasks = taskData?.subtasks || [];

  // Create subtask mutation
  const createSubtaskMutation = useMutation({
    mutationFn: (description) => api.createSubtask({
      taskId: task.taskId,
      description: description.trim(),
      createdBy: user.userId,
    }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['task', task.taskId] });
      setSubtaskDescription('');
      setShowSubtaskForm(false);
    },
  });

  const handleCreateSubtask = (e) => {
    e.preventDefault();
    if (!subtaskDescription.trim()) return;
    createSubtaskMutation.mutate(subtaskDescription);
  };

  // Update subtask mutation
  const updateSubtaskMutation = useMutation({
    mutationFn: ({ subtaskId, completedDate }) => api.updateSubtask(subtaskId, {
      completedDate: completedDate ? null : new Date().toISOString(),
    }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['task', task.taskId] });
    },
  });

  const handleToggleSubtask = (subtask) => {
    updateSubtaskMutation.mutate({
      subtaskId: subtask.subtaskId,
      completedDate: subtask.completedDate,
    });
  };

  const handleDeleteSubtaskClick = (subtaskId) => {
    setSubtaskToDelete(subtaskId);
    setShowDeleteSubtaskModal(true);
  };

  // Delete subtask mutation
  const deleteSubtaskMutation = useMutation({
    mutationFn: (subtaskId) => api.deleteSubtask(subtaskId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['task', task.taskId] });
      setShowDeleteSubtaskModal(false);
      setSubtaskToDelete(null);
    },
  });

  const handleConfirmDeleteSubtask = () => {
    if (!subtaskToDelete) return;
    deleteSubtaskMutation.mutate(subtaskToDelete);
  };

  // Update task mutation
  const updateTaskMutation = useMutation({
    mutationFn: (taskData) => api.updateTask(task.taskId, taskData),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['task', task.taskId] });
      queryClient.invalidateQueries({ queryKey: ['tasks', user.userId] });
      // Refetch and update parent
      queryClient.fetchQuery({
        queryKey: ['task', task.taskId],
        queryFn: () => api.getTask(task.taskId),
      }).then((fetchedTask) => {
        onTaskUpdate(fetchedTask);
      });
      setShowTaskForm(false);
    },
  });

  const handleTaskUpdate = (taskData) => {
    updateTaskMutation.mutate({
      ...taskData,
      completedDateProvided: false,
    });
  };

  const handleTaskComplete = () => {
    updateTaskMutation.mutate({
      completedDate: task.completedDate ? null : new Date().toISOString(),
      completedDateProvided: true,
    });
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
          initialTask={taskData || task}
          onSubmit={handleTaskUpdate}
          onCancel={() => setShowTaskForm(false)}
          loading={updateTaskMutation.isPending}
        />
      )}

      {isLoadingTask ? (
        <div className="loading-state">
          <p>Loading task details...</p>
        </div>
      ) : (
        <div className="task-detail-content">
          <div className="task-detail-title">
            <h1>{taskData?.taskName || task.taskName}</h1>
            {(taskData?.completedDate || task.completedDate) && (
              <span className="task-completed-badge">✓ Completed</span>
            )}
          </div>
          {(taskData?.description || task.description) && (
            <p className="task-detail-description">{taskData?.description || task.description}</p>
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
                <button type="submit" disabled={createSubtaskMutation.isPending} className="btn-primary btn-small">
                  {createSubtaskMutation.isPending ? 'Adding...' : 'Add'}
                </button>
              </div>
            </form>
          )}

          {(error || createSubtaskMutation.error || updateSubtaskMutation.error || deleteSubtaskMutation.error || updateTaskMutation.error) && (
            <div className="error-message">
              {error?.message || 
               createSubtaskMutation.error?.message || 
               updateSubtaskMutation.error?.message || 
               deleteSubtaskMutation.error?.message || 
               updateTaskMutation.error?.message || 
               'An error occurred'}
            </div>
          )}

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
                      disabled={updateSubtaskMutation.isPending}
                    />
                    <span>{subtask.description}</span>
                  </label>
                  <button
                    onClick={() => handleDeleteSubtaskClick(subtask.subtaskId)}
                    className="btn-icon"
                    disabled={deleteSubtaskMutation.isPending}
                  >
                    ×
                  </button>
                </div>
              ))
            )}
          </div>
        </div>
        </div>
      )}

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
        loading={updateTaskMutation.isPending}
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
        loading={deleteSubtaskMutation.isPending}
      />
    </div>
  );
}
