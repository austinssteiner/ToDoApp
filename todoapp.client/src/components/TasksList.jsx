import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../services/api';
import TaskForm from './TaskForm';
import TaskDetail from './TaskDetail';
import ConfirmModal from './ConfirmModal';

export default function TasksList({ user }) {
  const [showTaskForm, setShowTaskForm] = useState(false);
  const [selectedTask, setSelectedTask] = useState(null);
  const [showDeleteTaskModal, setShowDeleteTaskModal] = useState(false);
  const [taskToDelete, setTaskToDelete] = useState(null);
  const queryClient = useQueryClient();

  // Fetch tasks using React Query
  const { data: tasks = [], isLoading: initialLoading, error } = useQuery({
    queryKey: ['tasks', user.userId],
    queryFn: () => api.getTasks(user.userId),
    enabled: !!user?.userId,
  });

  // Create task mutation
  const createTaskMutation = useMutation({
    mutationFn: (taskData) => api.createTask({
      userId: user.userId,
      taskName: taskData.taskName,
      description: taskData.description,
      createdBy: user.userId,
    }),
    onSuccess: () => {
      // Invalidate and refetch tasks
      queryClient.invalidateQueries({ queryKey: ['tasks', user.userId] });
      setShowTaskForm(false);
    },
  });

  const handleCreateTask = (taskData) => {
    createTaskMutation.mutate(taskData);
  };

  const handleTaskClick = (task) => {
    setSelectedTask(task);
  };

  const handleBackToList = () => {
    setSelectedTask(null);
  };

  const handleTaskUpdate = async (updatedTask) => {
    // Invalidate both task detail and tasks list queries
    queryClient.invalidateQueries({ queryKey: ['task', updatedTask.taskId] });
    queryClient.invalidateQueries({ queryKey: ['tasks', user.userId] });
    
    // Refetch the updated task to get latest data including subtasks
    const fetchedTask = await queryClient.fetchQuery({
      queryKey: ['task', updatedTask.taskId],
      queryFn: () => api.getTask(updatedTask.taskId),
    });
    setSelectedTask(fetchedTask);
  };

  const handleTaskDeleteClick = (taskId) => {
    setTaskToDelete(taskId);
    setShowDeleteTaskModal(true);
  };

  // Delete task mutation
  const deleteTaskMutation = useMutation({
    mutationFn: (taskId) => api.deleteTask(taskId),
    onSuccess: () => {
      // Invalidate tasks list
      queryClient.invalidateQueries({ queryKey: ['tasks', user.userId] });
      // Invalidate task detail if it was the deleted task
      if (selectedTask?.taskId === taskToDelete) {
        queryClient.invalidateQueries({ queryKey: ['task', taskToDelete] });
        setSelectedTask(null);
      }
      setShowDeleteTaskModal(false);
      setTaskToDelete(null);
    },
  });

  const handleConfirmDeleteTask = () => {
    if (!taskToDelete) return;
    deleteTaskMutation.mutate(taskToDelete);
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

      {(error || createTaskMutation.error || deleteTaskMutation.error) && (
        <div className="error-message">
          {error?.message || createTaskMutation.error?.message || deleteTaskMutation.error?.message || 'An error occurred'}
        </div>
      )}

      {showTaskForm && (
        <TaskForm
          onSubmit={handleCreateTask}
          onCancel={() => setShowTaskForm(false)}
          loading={createTaskMutation.isPending}
        />
      )}

      {initialLoading ? (
        <div className="loading-state">
          <p>Loading tasks...</p>
        </div>
      ) : (
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
                <p className="task-description" title={task.description}>
                  {task.description.length > 160
                    ? `${task.description.slice(0, 157)}...`
                    : task.description}
                </p>
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
      )}

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
        loading={deleteTaskMutation.isPending}
      />
    </div>
  );
}
