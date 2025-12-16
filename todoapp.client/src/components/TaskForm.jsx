import { useState } from 'react';

export default function TaskForm({ onSubmit, onCancel, loading, initialTask = null }) {
  const [taskName, setTaskName] = useState(initialTask?.taskName || '');
  const [description, setDescription] = useState(initialTask?.description || '');

  const handleSubmit = (e) => {
    e.preventDefault();
    onSubmit({
      taskName: taskName.trim(),
      description: description.trim(),
    });
  };

  return (
    <div className="task-form-overlay">
      <div className="task-form-card">
        <h2>{initialTask ? 'Edit Task' : 'New Task'}</h2>
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="taskName">Task Name *</label>
            <input
              id="taskName"
              type="text"
              value={taskName}
              onChange={(e) => setTaskName(e.target.value)}
              required
              placeholder="Enter task name"
            />
          </div>
          <div className="form-group">
            <label htmlFor="description">Description</label>
            <textarea
              id="description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              placeholder="Enter task description"
              rows="4"
            />
          </div>
          <div className="form-actions">
            <button type="button" onClick={onCancel} className="btn-secondary">
              Cancel
            </button>
            <button type="submit" disabled={loading} className="btn-primary">
              {loading ? 'Saving...' : initialTask ? 'Update' : 'Create'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

